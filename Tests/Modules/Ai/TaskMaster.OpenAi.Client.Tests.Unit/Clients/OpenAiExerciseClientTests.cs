using NSubstitute;
using Shouldly;
using TaskMaster.Abstractions.Serialization;
using TaskMaster.Models.Exercises.Base;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests.OpenForm;
using TaskMaster.OpenAi.Client.Clients;
using TaskMaster.OpenAi.Client.Exceptions;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi.Client.Tests.Unit.Clients;

public sealed class OpenAiExerciseClientTests
{
    private readonly OpenAiExerciseClient _client;
    private readonly IPromptFormatter _promptFormatter;
    private readonly IObjectSamplerService _objectSamplerService;
    private readonly IOpenAiExerciseService _openAiExerciseService;
    private readonly ICustomSerializer _customSerializer;

    public OpenAiExerciseClientTests()
    {
        _promptFormatter = Substitute.For<IPromptFormatter>();
        _objectSamplerService = Substitute.For<IObjectSamplerService>();
        _openAiExerciseService = Substitute.For<IOpenAiExerciseService>();
        _customSerializer = Substitute.For<ICustomSerializer>();

        _client = new OpenAiExerciseClient(
            _promptFormatter,
            _objectSamplerService,
            _openAiExerciseService,
            _customSerializer);
    }

    #region Essay Tests

    [Fact]
    public async Task PromptForEssay_ShouldReturnEssay_WhenValidRequestProvided()
    {
        // Arrange
        var request = CreateValidEssayRequest();
        var cancellationToken = CancellationToken.None;
        
        var validationString = "request string";
        var validationStartMessage = "validation system message";
        var validationResponse = """{"isSuspicious": false, "reasons": []}""";
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        
        var startMessage = "system message";
        var exerciseJsonFormat = """{"header": {"title": "test"}}""";
        var openAiResponse = """{"header": {"title": "Environmental Protection Essay", "taskDescription": "Write about environment", "instruction": "Write 500 words", "example": "Start with...", "supportMaterial": "Consider..."}}""";
        var expectedEssay = CreateValidEssay();

        SetupMocksForEssay(request, validationString, validationStartMessage, validationResponse, 
            suspiciousPrompt, startMessage, exerciseJsonFormat, openAiResponse, expectedEssay, cancellationToken);

        // Act
        var result = await _client.PromptForEssay(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedEssay);
    }

    [Fact]
    public async Task PromptForEssay_ShouldThrowPromptInjectionException_WhenPromptIsSuspicious()
    {
        // Arrange
        var request = CreateValidEssayRequest();
        var cancellationToken = CancellationToken.None;
        
        var validationString = "malicious request";
        var validationStartMessage = "validation system message";
        var validationResponse = """{"isSuspicious": true, "reasons": ["Contains prompt injection"]}""";
        var suspiciousPrompt = new SuspiciousPrompt 
        { 
            IsSuspicious = true, 
            Reasons = ["Contains prompt injection"] 
        };

        _objectSamplerService.GetStringValues(request).Returns(validationString);
        _promptFormatter.FormatValidationSystemMessage().Returns(validationStartMessage);
        _openAiExerciseService.CompleteChatAsync(validationStartMessage, validationString, cancellationToken)
            .Returns(validationResponse);
        _customSerializer.TryDeserialize<SuspiciousPrompt>(validationResponse).Returns(suspiciousPrompt);

        // Act & Assert
        var exception = await Should.ThrowAsync<PromptInjectionException>(() => 
            _client.PromptForEssay(request, cancellationToken));
        exception.Message.ShouldContain("Contains prompt injection");
    }

    [Fact]
    public async Task PromptForEssay_ShouldThrowDeserializationException_WhenResponseCannotBeDeserialized()
    {
        // Arrange
        var request = CreateValidEssayRequest();
        var cancellationToken = CancellationToken.None;
        
        var validationString = "request string";
        var validationStartMessage = "validation system message";
        var validationResponse = """{"isSuspicious": false, "reasons": []}""";
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        
        var startMessage = "system message";
        var exerciseJsonFormat = """{"header": {"title": "test"}}""";
        var openAiResponse = "invalid json response";

        _objectSamplerService.GetStringValues(request).Returns(validationString);
        _promptFormatter.FormatValidationSystemMessage().Returns(validationStartMessage);
        _openAiExerciseService.CompleteChatAsync(validationStartMessage, validationString, cancellationToken)
            .Returns(validationResponse);
        _customSerializer.TryDeserialize<SuspiciousPrompt>(validationResponse).Returns(suspiciousPrompt);
        
        _promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage)
            .Returns(startMessage);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted data");
        _openAiExerciseService.CompleteChatAsync(startMessage, Arg.Any<string>(), cancellationToken)
            .Returns(openAiResponse);
        _customSerializer.TryDeserialize<Essay>(openAiResponse).Returns((Essay?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<DeserializationException>(() => 
            _client.PromptForEssay(request, cancellationToken));
        exception.Json.ShouldBe(openAiResponse);
    }

    [Fact]
    public async Task PromptForEssay_ShouldIncludeMinimumWordsInPrompt_WhenMinimumWordsSpecified()
    {
        // Arrange
        var request = CreateValidEssayRequest() with { MinimumNumberOfWords = 750 };
        var cancellationToken = CancellationToken.None;
        
        SetupBasicMocksForEssay(request, cancellationToken);

        // Act
        await _client.PromptForEssay(request, cancellationToken);

        // Assert
        await _openAiExerciseService.Received(1).CompleteChatAsync(
            Arg.Any<string>(),
            Arg.Is<string>(prompt => prompt.Contains("minimum number of words in essay - 750")),
            cancellationToken);
    }

    #endregion

    #region Mail Tests

    [Fact]
    public async Task PromptForMail_ShouldReturnMail_WhenValidRequestProvided()
    {
        // Arrange
        var request = CreateValidMailRequest();
        var cancellationToken = CancellationToken.None;
        
        var validationString = "request string";
        var validationStartMessage = "validation system message";
        var validationResponse = """{"isSuspicious": false, "reasons": []}""";
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        
        var startMessage = "system message";
        var exerciseJsonFormat = """{"header": {"title": "test"}}""";
        var openAiResponse = """{"header": {"title": "Business Email", "taskDescription": "Write business email", "instruction": "Write 300 words", "example": "Dear Sir...", "supportMaterial": "Be formal"}}""";
        var expectedMail = CreateValidMail();

        SetupMocksForMail(request, validationString, validationStartMessage, validationResponse, 
            suspiciousPrompt, startMessage, exerciseJsonFormat, openAiResponse, expectedMail, cancellationToken);

        // Act
        var result = await _client.PromptForMail(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedMail);
    }

    [Fact]
    public async Task PromptForMail_ShouldIncludeMinimumWordsInPrompt_WhenMinimumWordsSpecified()
    {
        // Arrange
        var request = CreateValidMailRequest() with { MinimumNumberOfWords = 250 };
        var cancellationToken = CancellationToken.None;
        
        SetupBasicMocksForMail(request, cancellationToken);

        // Act
        await _client.PromptForMail(request, cancellationToken);

        // Assert
        await _openAiExerciseService.Received(1).CompleteChatAsync(
            Arg.Any<string>(),
            Arg.Is<string>(prompt => prompt.Contains("minimum number of words in email - 250")),
            cancellationToken);
    }

    #endregion

    #region SummaryOfText Tests

    [Fact]
    public async Task PromptForSummaryOfText_ShouldReturnSummaryOfText_WhenValidRequestProvided()
    {
        // Arrange
        var request = CreateValidSummaryOfTextRequest();
        var cancellationToken = CancellationToken.None;
        
        var validationString = "request string";
        var validationStartMessage = "validation system message";
        var validationResponse = """{"isSuspicious": false, "reasons": []}""";
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        
        var startMessage = "system message";
        var exerciseJsonFormat = """{"header": {"title": "test"}, "textToSummary": "text"}""";
        var openAiResponse = """{"header": {"title": "Text Summary", "taskDescription": "Summarize text", "instruction": "Read and summarize", "example": "The text discusses...", "supportMaterial": "Key points"}, "textToSummary": "Long story about adventure..."}""";
        var expectedSummary = CreateValidSummaryOfText();

        SetupMocksForSummaryOfText(request, validationString, validationStartMessage, validationResponse, 
            suspiciousPrompt, startMessage, exerciseJsonFormat, openAiResponse, expectedSummary, cancellationToken);

        // Act
        var result = await _client.PromptForSummaryOfText(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedSummary);
    }

    [Fact]
    public async Task PromptForSummaryOfText_ShouldIncludeCorrectPromptType_WhenCalled()
    {
        // Arrange
        var request = CreateValidSummaryOfTextRequest();
        var cancellationToken = CancellationToken.None;
        
        SetupBasicMocksForSummaryOfText(request, cancellationToken);

        // Act
        await _client.PromptForSummaryOfText(request, cancellationToken);

        // Assert
        await _openAiExerciseService.Received(1).CompleteChatAsync(
            Arg.Any<string>(),
            Arg.Is<string>(prompt => prompt.Contains("summary of text exercise") && 
                                   prompt.Contains("generate a story (about 10 sentences)")),
            cancellationToken);
    }

    #endregion

    #region Helper Methods

    private static EssayRequestDto CreateValidEssayRequest()
    {
        return new EssayRequestDto
        {
            MinimumNumberOfWords = 500,
            ExerciseHeaderInMotherLanguage = true,
            MotherLanguage = "Polish",
            TargetLanguage = "English",
            TargetLanguageLevel = "Intermediate",
            TopicsOfSentences = "Environment",
            GrammarSection = "Present Perfect"
        };
    }

    private static MailRequestDto CreateValidMailRequest()
    {
        return new MailRequestDto
        {
            MinimumNumberOfWords = 300,
            ExerciseHeaderInMotherLanguage = false,
            MotherLanguage = "Polish",
            TargetLanguage = "English",
            TargetLanguageLevel = "Advanced",
            TopicsOfSentences = "Business",
            GrammarSection = "Formal language"
        };
    }

    private static SummaryOfTextRequestDto CreateValidSummaryOfTextRequest()
    {
        return new SummaryOfTextRequestDto
        {
            ExerciseHeaderInMotherLanguage = true,
            MotherLanguage = "Polish",
            TargetLanguage = "English",
            TargetLanguageLevel = "Beginner",
            TopicsOfSentences = "Adventure",
            GrammarSection = "Past tense"
        };
    }

    private static Essay CreateValidEssay()
    {
        return new Essay
        {
            Header = new Exercise.ExerciseHeader
            {
                Title = "Environmental Protection Essay",
                TaskDescription = "Write about environment",
                Instruction = "Write 500 words",
                Example = "Start with...",
                SupportMaterial = "Consider..."
            }
        };
    }

    private static Mail CreateValidMail()
    {
        return new Mail
        {
            Header = new Exercise.ExerciseHeader
            {
                Title = "Business Email",
                TaskDescription = "Write business email",
                Instruction = "Write 300 words",
                Example = "Dear Sir...",
                SupportMaterial = "Be formal"
            }
        };
    }

    private static SummaryOfText CreateValidSummaryOfText()
    {
        return new SummaryOfText
        {
            Header = new Exercise.ExerciseHeader
            {
                Title = "Text Summary",
                TaskDescription = "Summarize text",
                Instruction = "Read and summarize",
                Example = "The text discusses...",
                SupportMaterial = "Key points"
            },
            TextToSummary = "Long story about adventure..."
        };
    }

    private void SetupMocksForEssay(EssayRequestDto request, string validationString, string validationStartMessage, 
        string validationResponse, SuspiciousPrompt suspiciousPrompt, string startMessage, 
        string exerciseJsonFormat, string openAiResponse, Essay expectedEssay, CancellationToken cancellationToken)
    {
        // Validation mocks
        _objectSamplerService.GetStringValues(request).Returns(validationString);
        _promptFormatter.FormatValidationSystemMessage().Returns(validationStartMessage);
        _openAiExerciseService.CompleteChatAsync(validationStartMessage, validationString, cancellationToken)
            .Returns(validationResponse);
        _customSerializer.TryDeserialize<SuspiciousPrompt>(validationResponse).Returns(suspiciousPrompt);

        // Exercise generation mocks
        _promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage)
            .Returns(startMessage);
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted exercise data");
        _openAiExerciseService.CompleteChatAsync(startMessage, Arg.Any<string>(), cancellationToken)
            .Returns(openAiResponse);
        _customSerializer.TryDeserialize<Essay>(openAiResponse).Returns(expectedEssay);
    }

    private void SetupMocksForMail(MailRequestDto request, string validationString, string validationStartMessage, 
        string validationResponse, SuspiciousPrompt suspiciousPrompt, string startMessage, 
        string exerciseJsonFormat, string openAiResponse, Mail expectedMail, CancellationToken cancellationToken)
    {
        // Validation mocks
        _objectSamplerService.GetStringValues(request).Returns(validationString);
        _promptFormatter.FormatValidationSystemMessage().Returns(validationStartMessage);
        _openAiExerciseService.CompleteChatAsync(validationStartMessage, validationString, cancellationToken)
            .Returns(validationResponse);
        _customSerializer.TryDeserialize<SuspiciousPrompt>(validationResponse).Returns(suspiciousPrompt);

        // Exercise generation mocks
        _promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage)
            .Returns(startMessage);
        _objectSamplerService.GetSampleJson(typeof(Mail)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted exercise data");
        _openAiExerciseService.CompleteChatAsync(startMessage, Arg.Any<string>(), cancellationToken)
            .Returns(openAiResponse);
        _customSerializer.TryDeserialize<Mail>(openAiResponse).Returns(expectedMail);
    }

    private void SetupMocksForSummaryOfText(SummaryOfTextRequestDto request, string validationString, string validationStartMessage, 
        string validationResponse, SuspiciousPrompt suspiciousPrompt, string startMessage, 
        string exerciseJsonFormat, string openAiResponse, SummaryOfText expectedSummary, CancellationToken cancellationToken)
    {
        // Validation mocks
        _objectSamplerService.GetStringValues(request).Returns(validationString);
        _promptFormatter.FormatValidationSystemMessage().Returns(validationStartMessage);
        _openAiExerciseService.CompleteChatAsync(validationStartMessage, validationString, cancellationToken)
            .Returns(validationResponse);
        _customSerializer.TryDeserialize<SuspiciousPrompt>(validationResponse).Returns(suspiciousPrompt);

        // Exercise generation mocks
        _promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage)
            .Returns(startMessage);
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns(exerciseJsonFormat);
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted exercise data");
        _openAiExerciseService.CompleteChatAsync(startMessage, Arg.Any<string>(), cancellationToken)
            .Returns(openAiResponse);
        _customSerializer.TryDeserialize<SummaryOfText>(openAiResponse).Returns(expectedSummary);
    }

    private void SetupBasicMocksForEssay(EssayRequestDto request, CancellationToken cancellationToken)
    {
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        var expectedEssay = CreateValidEssay();
        
        _objectSamplerService.GetStringValues(request).Returns("request string");
        _promptFormatter.FormatValidationSystemMessage().Returns("validation message");
        _openAiExerciseService.CompleteChatAsync(Arg.Any<string>(), Arg.Any<string>(), cancellationToken)
            .Returns("""{"isSuspicious": false, "reasons": []}""", "essay response");
        _customSerializer.TryDeserialize<SuspiciousPrompt>(Arg.Any<string>()).Returns(suspiciousPrompt);
        _customSerializer.TryDeserialize<Essay>(Arg.Any<string>()).Returns(expectedEssay);
        
        _promptFormatter.FormatStartingSystemMessage(Arg.Any<string>(), Arg.Any<string>()).Returns("system message");
        _objectSamplerService.GetSampleJson(typeof(Essay)).Returns("json format");
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted data");
    }

    private void SetupBasicMocksForMail(MailRequestDto request, CancellationToken cancellationToken)
    {
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        var expectedMail = CreateValidMail();
        
        _objectSamplerService.GetStringValues(request).Returns("request string");
        _promptFormatter.FormatValidationSystemMessage().Returns("validation message");
        _openAiExerciseService.CompleteChatAsync(Arg.Any<string>(), Arg.Any<string>(), cancellationToken)
            .Returns("""{"isSuspicious": false, "reasons": []}""", "mail response");
        _customSerializer.TryDeserialize<SuspiciousPrompt>(Arg.Any<string>()).Returns(suspiciousPrompt);
        _customSerializer.TryDeserialize<Mail>(Arg.Any<string>()).Returns(expectedMail);
        
        _promptFormatter.FormatStartingSystemMessage(Arg.Any<string>(), Arg.Any<string>()).Returns("system message");
        _objectSamplerService.GetSampleJson(typeof(Mail)).Returns("json format");
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted data");
    }

    private void SetupBasicMocksForSummaryOfText(SummaryOfTextRequestDto request, CancellationToken cancellationToken)
    {
        var suspiciousPrompt = new SuspiciousPrompt { IsSuspicious = false, Reasons = [] };
        var expectedSummary = CreateValidSummaryOfText();
        
        _objectSamplerService.GetStringValues(request).Returns("request string");
        _promptFormatter.FormatValidationSystemMessage().Returns("validation message");
        _openAiExerciseService.CompleteChatAsync(Arg.Any<string>(), Arg.Any<string>(), cancellationToken)
            .Returns("""{"isSuspicious": false, "reasons": []}""", "summary response");
        _customSerializer.TryDeserialize<SuspiciousPrompt>(Arg.Any<string>()).Returns(suspiciousPrompt);
        _customSerializer.TryDeserialize<SummaryOfText>(Arg.Any<string>()).Returns(expectedSummary);
        
        _promptFormatter.FormatStartingSystemMessage(Arg.Any<string>(), Arg.Any<string>()).Returns("system message");
        _objectSamplerService.GetSampleJson(typeof(SummaryOfText)).Returns("json format");
        _promptFormatter.FormatExerciseBaseData(request).Returns("formatted data");
    }

    #endregion
}