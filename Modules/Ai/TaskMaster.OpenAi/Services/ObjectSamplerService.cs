using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Schema;
using TaskMaster.Models.Exercises.Base;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.OpenAi.OpenForm;

namespace TaskMaster.OpenAi.Services;

internal sealed class ObjectSamplerService : IObjectSamplerService
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string GetSampleJson(Type type)
    {
        var sample = GenerateSampleObject(type);
        return JsonSerializer.Serialize(sample, Options);
    }

    public string GetStringValues(object? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var sb = new StringBuilder();

        foreach (var property in properties)
        {
            if (property.PropertyType != typeof(string))
                continue;

            var value = property.GetValue(obj) as string;

            if (string.IsNullOrWhiteSpace(value))
                continue;

            sb.AppendLine(value);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Only for types with default constructor!
    /// </summary>
    private static object GenerateSampleObject(Type type)
    {
        if (type == typeof(string))
            return "string";

        if (type.IsValueType)
            return Activator.CreateInstance(type)!;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var listType = type.GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance(type)!;
            list.Add(GenerateSampleObject(listType));
            return list;
        }

        var obj = Activator.CreateInstance(type);
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            if (property.CanWrite)
                property.SetValue(obj, GenerateSampleObject(property.PropertyType));

        return obj!;
    }
}