using System.Text.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using TaskMaster.Abstractions.Events;
using TaskMaster.Abstractions.Serialization;
using TaskMaster.Events.Exercises.OpenForm;
using TaskMaster.Events.SupiciousPrompts;
using TaskMaster.Models.Exercises.Base;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.OpenAi.Exceptions;
using TaskMaster.OpenAi.OpenForm.Queries;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi.Tests.Unit.OpenForm.Queries;

public sealed class EssayQueryHandlerTests
{
    private readonly EssayQueryHandler _handler;
    private readonly IPromptFormatter _promptFormatter;
    private readonly IObjectSamplerService _objectSamplerService;
    private readonly IOpenAiExerciseService _openAiExerciseService;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICustomSerializer _customSerializer;

    public EssayQueryHandlerTests()
    {
        _promptFormatter = Substitute.For<IPromptFormatter>();
        _objectSamplerService = Substitute.For<IObjectSamplerService>();
        _openAiExerciseService = Substitute.For<IOpenAiExerciseService>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _customSerializer = Substitute.For<ICustomSerializer>();

        _handler = new EssayQueryHandler(
            _promptFormatter,
            _objectSamplerService,
            _openAiExerciseService,
            _eventDispatcher,
            _customSerializer);
    }

    #region Happy Path Tests

    [Fact]
    public async Task HandleAsync_ShouldReturnEssayResponseOpenForm_WhenValidQueryProvided()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Environmental Protection Essay",
                "taskDescription": "Write an essay about environmental protection",
                "instruction": "Write at least 500 words about environmental protection measures",
                "example": "Start with an introduction paragraph...",
                "supportMaterial": "Consider climate change impacts..."
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<EssayResponseOpenForm>();
        result.Exercise.ShouldNotBeNull();
        result.Exercise.Header.Title.ShouldBe("Environmental Protection Essay");
        result.Exercise.Header.TaskDescription.ShouldBe("Write an essay about environmental protection");
        result.Exercise.Header.Instruction.ShouldBe("Write at least 500 words about environmental protection measures");
        result.MotherLanguage.ShouldBe(query.MotherLanguage);
        result.TargetLanguage.ShouldBe(query.TargetLanguage);
        result.TargetLanguageLevel.ShouldBe(query.TargetLanguageLevel);
        result.ExerciseHeaderInMotherLanguage.ShouldBe(query.ExerciseHeaderInMotherLanguage);
        result.TopicsOfSentences.ShouldBe(query.TopicsOfSentences);
        result.GrammarSection.ShouldBe(query.GrammarSection);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishOpenFormGeneratedEvent_WhenEssayCreatedSuccessfully()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        await _eventDispatcher.Received(1).PublishAsync(
            Arg.Is<OpenFormGenerated<Essay>>(e => 
                e.Id == result.Id &&
                e.Exercise.Header.Title == "Test Essay" &&
                e.ExerciseHeaderInMotherLanguage == query.ExerciseHeaderInMotherLanguage &&
                e.MotherLanguage == query.MotherLanguage &&
                e.TargetLanguage == query.TargetLanguage &&
                e.TargetLanguageLevel == query.TargetLanguageLevel &&
                e.TopicsOfSentences == query.TopicsOfSentences &&
                e.GrammarSection == query.GrammarSection));
    }

    #endregion

    #region Prompt Injection Tests

    [Fact]
    public async Task HandleAsync_ShouldThrowPromptInjectionException_WhenPromptIsSuspicious()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "malicious query string";
        var suspiciousPromptResponse = new SuspiciousPrompt 
        { 
            IsSuspicious = true, 
            Reasons = new List<string> { "Contains prompt injection", "Attempts to bypass filters" }
        };

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);

        // Act & Assert
        var exception = await Should.ThrowAsync<PromptInjectionException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldContain("Contains prompt injection");
        exception.Message.ShouldContain("Attempts to bypass filters");
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishSuspiciousPromptInjectedEvent_WhenPromptIsSuspicious()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "malicious query string";
        var reasons = new List<string> { "Contains prompt injection", "Attempts to bypass filters" };
        var suspiciousPromptResponse = new SuspiciousPrompt 
        { 
            IsSuspicious = true, 
            Reasons = reasons
        };

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);

        // Act & Assert
        await Should.ThrowAsync<PromptInjectionException>(() => _handler.HandleAsync(query));
        
        await _eventDispatcher.Received(1).PublishAsync(
            Arg.Is<SuspiciousPromptInjected>(e => 
                e.Reasons.SequenceEqual(reasons)));
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPublishSuspiciousPromptEvent_WhenPromptIsNotSuspicious()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "safe query string";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        await _eventDispatcher.DidNotReceive().PublishAsync(Arg.Any<SuspiciousPromptInjected>());
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public async Task HandleAsync_ShouldThrowDeserializationException_WhenCustomSerializerReturnsNull()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var invalidJsonResponse = "{ invalid json }";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(invalidJsonResponse);
        _customSerializer.TryDeserialize<Essay>(invalidJsonResponse).Returns((Essay?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("There was a error during response deserialization");
        exception.Json.ShouldBe(invalidJsonResponse);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowDeserializationException_WhenCustomSerializerReturnsNullForValidJson()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var validJsonResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(validJsonResponse);
        _customSerializer.TryDeserialize<Essay>(validJsonResponse).Returns((Essay?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("There was a error during response deserialization");
        exception.Json.ShouldBe(validJsonResponse);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowDeserializationException_WhenCustomSerializerReturnsNullForEmptyResponse()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var emptyJsonResponse = "";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(emptyJsonResponse);
        _customSerializer.TryDeserialize<Essay>(emptyJsonResponse).Returns((Essay?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("There was a error during response deserialization");
        exception.Json.ShouldBe(emptyJsonResponse);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenCustomSerializerThrows()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var jsonResponse = "{ \"some\": \"json\" }";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(jsonResponse);
        _customSerializer.TryDeserialize<Essay>(jsonResponse).Throws(new JsonException("Serialization error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<JsonException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("Serialization error");
    }

    #endregion

    #region Service Dependency Tests

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenObjectSamplerServiceThrows()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        _objectSamplerService.GetStringValues(query).Throws(new InvalidOperationException("Sampling failed"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("Sampling failed");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenOpenAiServiceValidationThrows()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        
        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Throws(new HttpRequestException("API unavailable"));

        // Act & Assert
        var exception = await Should.ThrowAsync<HttpRequestException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("API unavailable");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenOpenAiServicePromptForExerciseThrows()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Throws(new HttpRequestException("OpenAI API error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<HttpRequestException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("OpenAI API error");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenEventDispatcherThrows()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);
        _eventDispatcher.PublishAsync(Arg.Any<OpenFormGenerated<Essay>>()).Throws(new InvalidOperationException("Event dispatch failed"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("Event dispatch failed");
    }

    [Fact]
    public async Task HandleAsync_ShouldCallCustomSerializerCorrectly_WhenDeserializingResponse()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        _customSerializer.Received(1).TryDeserialize<Essay>(openAiResponse);
    }

    #endregion

    #region Prompt Construction Tests

    [Fact]
    public async Task HandleAsync_ShouldConstructPromptCorrectly_WhenMinimumWordsSpecified()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        query.MinimumNumberOfWords = 750;
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted base data";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        await _openAiExerciseService.Received(1).PromptForExercise(
            Arg.Is<string>(prompt => 
                prompt.Contains("1. This is open form - essay exercise") &&
                prompt.Contains(formattedPrompt) &&
                prompt.Contains("12. In instruction field include information about the minimum number of words in essay - 750") &&
                prompt.Contains("13. Your responses should be structured in JSON format") &&
                prompt.Contains(exerciseJsonFormat)),
            query.MotherLanguage, 
            query.TargetLanguage);
    }

    [Fact]
    public async Task HandleAsync_ShouldUseCorrectServiceCallOrder_WhenProcessingQuery()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert - Verify order of service calls
        Received.InOrder(() => {
            _objectSamplerService.GetStringValues(query);
            _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString);
            _objectSamplerService.GetSampleJson(typeof(Essay));
            _promptFormatter.FormatExerciseBaseData(query);
            _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage);
            _customSerializer.TryDeserialize<Essay>(Arg.Any<string>());
            _eventDispatcher.PublishAsync(Arg.Any<OpenFormGenerated<Essay>>());
        });
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task HandleAsync_ShouldHandleZeroMinimumWords_WhenMinimumWordsIsZero()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        query.MinimumNumberOfWords = 0;
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        await _openAiExerciseService.Received(1).PromptForExercise(
            Arg.Is<string>(prompt => prompt.Contains("minimum number of words in essay - 0")),
            query.MotherLanguage, 
            query.TargetLanguage);
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleNegativeMinimumWords_WhenMinimumWordsIsNegative()
    {
        // Arrange
        var query = CreateValidEssayQuery();
        query.MinimumNumberOfWords = -100;
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        await _openAiExerciseService.Received(1).PromptForExercise(
            Arg.Is<string>(prompt => prompt.Contains("minimum number of words in essay - -100")),
            query.MotherLanguage, 
            query.TargetLanguage);
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleNullOptionalFields_WhenOptionalFieldsAreNull()
    {
        // Arrange
        var query = new EssayQuery
        {
            MinimumNumberOfWords = 500,
            ExerciseHeaderInMotherLanguage = true,
            MotherLanguage = "English",
            TargetLanguage = "Spanish",
            TargetLanguageLevel = "Intermediate",
            TopicsOfSentences = null,
            GrammarSection = null,
            SupportMaterial = null
        };
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Essay",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            }
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.TopicsOfSentences.ShouldBeNull();
        result.GrammarSection.ShouldBeNull();
    }

    #endregion

    #region Helper Methods

    private EssayQuery CreateValidEssayQuery()
    {
        return new EssayQuery
        {
            MinimumNumberOfWords = 500,
            ExerciseHeaderInMotherLanguage = true,
            MotherLanguage = "English",
            TargetLanguage = "Spanish",
            TargetLanguageLevel = "Intermediate",
            TopicsOfSentences = "Environment, Technology",
            GrammarSection = "Present Perfect Tense",
            SupportMaterial = "Additional resources"
        };
    }

    private void SetupMocks(EssayQuery query, string queryAsString, SuspiciousPrompt suspiciousPromptResponse, 
        string exerciseJsonFormat, string formattedPrompt, string openAiResponse)
    {
        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(openAiResponse);
        
        // Create a mock Essay object that matches the JSON structure
        var parsedJson = JsonSerializer.Deserialize<JsonElement>(openAiResponse);
        var header = parsedJson.GetProperty("header");
        
        var essay = new Essay
        {
            Header = new Exercise.ExerciseHeader
            {
                Title = header.GetProperty("title").GetString() ?? "Test Essay",
                TaskDescription = header.GetProperty("taskDescription").GetString() ?? "Description", 
                Instruction = header.GetProperty("instruction").GetString() ?? "Instructions",
                Example = header.GetProperty("example").GetString() ?? "Example",
                SupportMaterial = header.GetProperty("supportMaterial").GetString() ?? "Support"
            }
        };
        _customSerializer.TryDeserialize<Essay>(openAiResponse).Returns(essay);
    }

    #endregion
}