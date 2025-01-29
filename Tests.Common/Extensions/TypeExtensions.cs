using System.Reflection;

namespace Tests.Common.Extensions;

internal static class TypeExtensions
{
    public static IEnumerable<PropertyInfo> GetPublicOrInternalProperties(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(property
                => property.GetMethod?.IsPublic is true
                || property.GetMethod?.IsAssembly is true
            );
    }
}
