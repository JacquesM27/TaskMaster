namespace TaskMaster.OpenAi.Services;

internal interface IObjectSamplerService
{
    //string GetStaticJsonSchema(Type type);
    string GetSampleJson(Type type);
    string GetStringValues(object? obj);
}