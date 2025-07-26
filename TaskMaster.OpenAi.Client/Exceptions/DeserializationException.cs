namespace TaskMaster.OpenAi.Client.Exceptions;

public sealed class DeserializationException(string json)
    : Exception("There was a error during response deserialization")
{
    public string Json => json;
}