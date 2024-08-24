namespace TaskMaster.Infrastructure.ReflectionExtensions;

public static class TypesExtensions
{
    public static IEnumerable<Type> GetNonAbstractDerivedTypes<T>()
    {
        var baseType = typeof(T);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        return assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract);
    }

    public static IEnumerable<string> GetNonAbstractDerivedTypeNames<T>()
    {
        return GetNonAbstractDerivedTypes<T>().Select(t => t.Name);
    }
}