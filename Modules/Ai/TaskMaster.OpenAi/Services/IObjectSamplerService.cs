namespace TaskMaster.OpenAi.Services;

public interface IObjectSamplerService
{
    string GetSampleJson(Type type);
    string GetStringValues(object? obj);
}