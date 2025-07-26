using TaskMaster.Abstractions.Serialization;
using TaskMaster.Models.Exercises.Base;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Exercises.Requests;
using TaskMaster.Models.Exercises.Requests.OpenForm;
using TaskMaster.OpenAi.Client.Exceptions;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi.Client.Clients;

internal sealed class OpenAiExerciseClient(
    IPromptFormatter promptFormatter,
    IObjectSamplerService objectSamplerService,
    IOpenAiExerciseService openAiExerciseService,
    ICustomSerializer customSerializer) : IOpenAiExerciseClient
{
    public async Task<Essay> PromptForEssay(EssayRequestDto request)
    {
        await ValidateAvoidingOriginTopic(request);
        
        var startMessage = promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage);
        var exerciseJsonFormat = objectSamplerService.GetSampleJson(typeof(Essay));
        var prompt =
            "1. This is open form - essay exercise. This means that you need to generate a short essay topic to be written by the student.";
        prompt += promptFormatter.FormatExerciseBaseData(request);
        prompt +=
            $"12. In instruction field include information about the minimum number of words in essay - {request.MinimumNumberOfWords}. It's important.\n";
        prompt += $"""
                   13. Your responses should be structured in JSON format as follows:
                   {exerciseJsonFormat}
                   """;
        var response =
            await openAiExerciseService.CompleteChatAsync(startMessage, prompt);
        var exercise = customSerializer.TryDeserialize<Essay>(response)
                       ?? throw new DeserializationException(response);
        return exercise;
    }

    public async Task<Mail> PromptForMail(MailRequestDto request)
    {
        await ValidateAvoidingOriginTopic(request);
        
        var startMessage = promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage);
        var exerciseJsonFormat = objectSamplerService.GetSampleJson(typeof(Mail));
        var prompt =
            "1. This is open form - mail exercise. This means that you need to generate a short description of the email to be written by the student. Add information on who the email should be to.";
        prompt += promptFormatter.FormatExerciseBaseData(request);
        prompt +=
            $"12. In instruction field include information about the minimum number of words in email - {request.MinimumNumberOfWords}. It's important.\n";
        prompt += $"""
                   13. Your responses should be structured in JSON format as follows:
                   {exerciseJsonFormat}
                   """;
        var response =
            await openAiExerciseService.CompleteChatAsync(startMessage, prompt);
        var exercise = customSerializer.TryDeserialize<Mail>(response)
                       ?? throw new DeserializationException(response);
        return exercise;
    }

    public async Task<SummaryOfText> PromptForSummaryOfText(SummaryOfTextRequestDto request)
    {
        await ValidateAvoidingOriginTopic(request);
        
        var startMessage = promptFormatter.FormatStartingSystemMessage(request.MotherLanguage, request.TargetLanguage);
        var exerciseJsonFormat = objectSamplerService.GetSampleJson(typeof(SummaryOfText));
        var prompt =
            "1. This is open form - summary of text exercise. This means that you need to generate a story (about 10 sentences) to be summarized by the student.";
        prompt += promptFormatter.FormatExerciseBaseData(request);
        prompt += $"""
                   12. Your responses should be structured in JSON format as follows:
                   {exerciseJsonFormat}
                   """;
        var response =
            await openAiExerciseService.CompleteChatAsync(startMessage, prompt);
        var exercise = customSerializer.TryDeserialize<SummaryOfText>(response)
                       ?? throw new DeserializationException(response);
        return exercise;
    }

    private async Task ValidateAvoidingOriginTopic<TRequest>(TRequest request)
        where TRequest : ExerciseRequestBase
    {
        var queryAsString = objectSamplerService.GetStringValues(request);
        var validationStartMessage = promptFormatter.FormatValidationSystemMessage();
        var validationResponse = await openAiExerciseService.CompleteChatAsync(validationStartMessage, queryAsString);
        var validationResult = customSerializer.TryDeserialize<SuspiciousPrompt>(validationResponse)
                               ?? throw new DeserializationException(validationResponse);
        if (validationResult.IsSuspicious)
        {
            throw new PromptInjectionException(validationResult.Reasons);
        }
    }
}