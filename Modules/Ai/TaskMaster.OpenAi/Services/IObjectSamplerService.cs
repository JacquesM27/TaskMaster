namespace TaskMaster.OpenAi.Services;

internal interface IObjectSamplerService
{
    string GetSampleJson(Type type);
    string GetStringValues(object? obj);
}