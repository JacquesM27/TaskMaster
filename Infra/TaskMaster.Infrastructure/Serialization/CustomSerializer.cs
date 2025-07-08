using System.Text.Json;
using TaskMaster.Abstractions.Serialization;

namespace TaskMaster.Infrastructure.Serialization;

public class CustomSerializer : ICustomSerializer
{
    private static readonly JsonSerializerOptions DefaultDeserializationOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public T? TryDeserialize<T>(string json) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultDeserializationOptions);
        }
        catch (Exception)
        {
            return null;
        }
    }
}