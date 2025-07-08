namespace TaskMaster.Abstractions.Serialization;

public interface ICustomSerializer
{
    T? TryDeserialize<T>(string json) where T : class;
}