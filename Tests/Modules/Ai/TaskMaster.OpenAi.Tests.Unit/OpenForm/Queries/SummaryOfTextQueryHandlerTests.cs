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

public sealed class SummaryOfTextQueryHandlerTests
{
    private readonly SummaryOfTextQueryHandler _handler;
    private readonly IPromptFormatter _promptFormatter;
    private readonly IObjectSamplerService _objectSamplerService;
    private readonly IOpenAiExerciseService _openAiExerciseService;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICustomSerializer _customSerializer;

    public SummaryOfTextQueryHandlerTests()
    {
        _promptFormatter = Substitute.For<IPromptFormatter>();
        _objectSamplerService = Substitute.For<IObjectSamplerService>();
        _openAiExerciseService = Substitute.For<IOpenAiExerciseService>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _customSerializer = Substitute.For<ICustomSerializer>();

        _handler = new SummaryOfTextQueryHandler(
            _promptFormatter,
            _objectSamplerService,
            _openAiExerciseService,
            _eventDispatcher,
            _customSerializer);
    }

    #region Happy Path Tests

    [Fact]
    public async Task HandleAsync_ShouldReturnSummaryOfTextResponseOpenForm_WhenValidQueryProvided()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Text Summarization Exercise",
                "taskDescription": "Summarize the given text",
                "instruction": "Read the text carefully and write a summary in your own words",
                "example": "The story tells about...",
                "supportMaterial": "Focus on main ideas and key points..."
            },
            "textToSummary": "Once upon a time, in a small village nestled between rolling hills and a crystal-clear river, there lived a young baker named Elena. Every morning, she would wake before dawn to prepare fresh bread for the villagers. The aroma of baking bread filled the air, drawing people from their homes to her small bakery. Elena took great pride in her work, carefully selecting the finest ingredients and kneading each loaf with love. Her breads were not just food; they were a symbol of community and warmth. The villagers would gather at her bakery, sharing stories and laughter over warm pastries. Elena's dedication to her craft and her community made her beloved by all who knew her."
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<SummaryOfTextResponseOpenForm>();
        result.Exercise.ShouldNotBeNull();
        result.Exercise.Header.Title.ShouldBe("Text Summarization Exercise");
        result.Exercise.Header.TaskDescription.ShouldBe("Summarize the given text");
        result.Exercise.Header.Instruction.ShouldBe("Read the text carefully and write a summary in your own words");
        result.Exercise.TextToSummary.ShouldContain("Once upon a time");
        result.MotherLanguage.ShouldBe(query.MotherLanguage);
        result.TargetLanguage.ShouldBe(query.TargetLanguage);
        result.TargetLanguageLevel.ShouldBe(query.TargetLanguageLevel);
        result.ExerciseHeaderInMotherLanguage.ShouldBe(query.ExerciseHeaderInMotherLanguage);
        result.TopicsOfSentences.ShouldBe(query.TopicsOfSentences);
        result.GrammarSection.ShouldBe(query.GrammarSection);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishOpenFormGeneratedEvent_WhenSummaryOfTextCreatedSuccessfully()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized by the student."
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        await _eventDispatcher.Received(1).PublishAsync(
            Arg.Is<OpenFormGenerated<SummaryOfText>>(e => 
                e.Id == result.Id &&
                e.Exercise.Header.Title == "Test Summary" &&
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
        var query = CreateValidSummaryOfTextQuery();
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
        var query = CreateValidSummaryOfTextQuery();
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
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "safe query string";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
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
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var invalidJsonResponse = "{ invalid json }";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(invalidJsonResponse);
        _customSerializer.TryDeserialize<SummaryOfText>(invalidJsonResponse).Returns((SummaryOfText?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("There was a error during response deserialization");
        exception.Json.ShouldBe(invalidJsonResponse);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowDeserializationException_WhenCustomSerializerReturnsNullForValidJson()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var validJsonResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
        }
        """;

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(validJsonResponse);
        _customSerializer.TryDeserialize<SummaryOfText>(validJsonResponse).Returns((SummaryOfText?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("There was a error during response deserialization");
        exception.Json.ShouldBe(validJsonResponse);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowDeserializationException_WhenCustomSerializerReturnsNullForEmptyResponse()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var emptyJsonResponse = "";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(emptyJsonResponse);
        _customSerializer.TryDeserialize<SummaryOfText>(emptyJsonResponse).Returns((SummaryOfText?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("There was a error during response deserialization");
        exception.Json.ShouldBe(emptyJsonResponse);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenCustomSerializerThrows()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var jsonResponse = "{ \"some\": \"json\" }";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(jsonResponse);
        _customSerializer.TryDeserialize<SummaryOfText>(jsonResponse).Throws(new JsonException("Serialization error"));

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
        var query = CreateValidSummaryOfTextQuery();
        _objectSamplerService.GetStringValues(query).Throws(new InvalidOperationException("Sampling failed"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("Sampling failed");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenOpenAiServiceValidationThrows()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
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
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";

        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
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
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);
        _eventDispatcher.PublishAsync(Arg.Any<OpenFormGenerated<SummaryOfText>>()).Throws(new InvalidOperationException("Event dispatch failed"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _handler.HandleAsync(query));
        exception.Message.ShouldBe("Event dispatch failed");
    }

    [Fact]
    public async Task HandleAsync_ShouldCallCustomSerializerCorrectly_WhenDeserializingResponse()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        _customSerializer.Received(1).TryDeserialize<SummaryOfText>(openAiResponse);
    }

    #endregion

    #region Prompt Construction Tests

    [Fact]
    public async Task HandleAsync_ShouldConstructPromptCorrectly_WhenGeneratingExercise()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted base data";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        await _openAiExerciseService.Received(1).PromptForExercise(
            Arg.Is<string>(prompt => 
                prompt.Contains("1. This is open form - summary of text exercise. This means that you need to generate a story (about 10 sentences) to be summarized by the student.") &&
                prompt.Contains(formattedPrompt) &&
                prompt.Contains("12. Your responses should be structured in JSON format") &&
                prompt.Contains(exerciseJsonFormat)),
            query.MotherLanguage, 
            query.TargetLanguage);
    }

    [Fact]
    public async Task HandleAsync_ShouldUseCorrectServiceCallOrder_WhenProcessingQuery()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        await _handler.HandleAsync(query);

        // Assert - Verify order of service calls
        Received.InOrder(() => {
            _objectSamplerService.GetStringValues(query);
            _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString);
            _objectSamplerService.GetSampleJson(typeof(SummaryOfText));
            _promptFormatter.FormatExerciseBaseData(query);
            _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage);
            _customSerializer.TryDeserialize<SummaryOfText>(Arg.Any<string>());
            _eventDispatcher.PublishAsync(Arg.Any<OpenFormGenerated<SummaryOfText>>());
        });
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task HandleAsync_ShouldHandleNullOptionalFields_WhenOptionalFieldsAreNull()
    {
        // Arrange
        var query = new SummaryOfTextQuery
        {
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
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "Sample text to be summarized."
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

    [Fact]
    public async Task HandleAsync_ShouldHandleEmptyTextToSummary_WhenTextToSummaryIsEmpty()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var openAiResponse = """
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": ""
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Exercise.TextToSummary.ShouldBe("");
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleLongTextToSummary_WhenTextToSummaryIsVeryLong()
    {
        // Arrange
        var query = CreateValidSummaryOfTextQuery();
        var queryAsString = "query string representation";
        var suspiciousPromptResponse = new SuspiciousPrompt { IsSuspicious = false };
        var exerciseJsonFormat = "{ \"header\": { \"title\": \"Test\" } }";
        var formattedPrompt = "formatted prompt";
        var longText = string.Join(" ", Enumerable.Repeat("This is a very long text that needs to be summarized by the student.", 50));
        var openAiResponse = $$"""
        {
            "header": {
                "title": "Test Summary",
                "taskDescription": "Description",
                "instruction": "Instructions",
                "example": "Example",
                "supportMaterial": "Support"
            },
            "textToSummary": "{{longText}}"
        }
        """;

        SetupMocks(query, queryAsString, suspiciousPromptResponse, exerciseJsonFormat, formattedPrompt, openAiResponse);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Exercise.TextToSummary.ShouldBe(longText);
    }

    #endregion

    #region Helper Methods

    private SummaryOfTextQuery CreateValidSummaryOfTextQuery()
    {
        return new SummaryOfTextQuery
        {
            ExerciseHeaderInMotherLanguage = true,
            MotherLanguage = "English",
            TargetLanguage = "Spanish",
            TargetLanguageLevel = "Intermediate",
            TopicsOfSentences = "Stories, Literature",
            GrammarSection = "Past Tense",
            SupportMaterial = "Focus on main ideas and key details"
        };
    }

    private void SetupMocks(SummaryOfTextQuery query, string queryAsString, SuspiciousPrompt suspiciousPromptResponse, 
        string exerciseJsonFormat, string formattedPrompt, string openAiResponse)
    {
        _objectSamplerService.GetStringValues(query).Returns(queryAsString);
        _openAiExerciseService.ValidateAvoidingOriginTopic(queryAsString).Returns(suspiciousPromptResponse);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(query).Returns(formattedPrompt);
        _openAiExerciseService.PromptForExercise(Arg.Any<string>(), query.MotherLanguage, query.TargetLanguage)
            .Returns(openAiResponse);
        
        // Create a mock SummaryOfText object that matches the JSON structure
        var parsedJson = JsonSerializer.Deserialize<JsonElement>(openAiResponse);
        var header = parsedJson.GetProperty("header");
        var textToSummary = parsedJson.GetProperty("textToSummary").GetString() ?? "";
        
        var summaryOfText = new SummaryOfText
        {
            Header = new Exercise.ExerciseHeader
            {
                Title = header.GetProperty("title").GetString() ?? "Test Summary",
                TaskDescription = header.GetProperty("taskDescription").GetString() ?? "Description", 
                Instruction = header.GetProperty("instruction").GetString() ?? "Instructions",
                Example = header.GetProperty("example").GetString() ?? "Example",
                SupportMaterial = header.GetProperty("supportMaterial").GetString() ?? "Support"
            },
            TextToSummary = textToSummary
        };
        _customSerializer.TryDeserialize<SummaryOfText>(openAiResponse).Returns(summaryOfText);
    }

    #endregion
}