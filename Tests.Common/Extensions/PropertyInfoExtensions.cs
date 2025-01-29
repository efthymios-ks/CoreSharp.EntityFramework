using System.Reflection;

namespace Tests.Common.Extensions;

internal static class PropertyInfoExtensions
{
    public static bool IsMockable(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);

        // No getter - Skip
        if (!property.CanRead)
        {
            return false;
        }

        // Indexer - Skip
        if (property.GetIndexParameters().Length > 0)
        {
            return false;
        }

        // Interface - Pass directly
        if (property.DeclaringType!.IsInterface)
        {
            return true;
        }

        // Abstract - Further check
        if (property.DeclaringType!.IsAbstract)
        {
            var getMethod = property.GetMethod;

            // No getter - Skip
            if (getMethod is null)
            {
                return false;
            }

            // No backing fields, propably expression body - Skip
            var getMethodBody = getMethod.GetMethodBody();
            if (getMethodBody is null || !getMethodBody.LocalVariables.Any())
            {
                return false;
            }

            return true;
        }

        // Not interface or abstract - Skip
        return false;
    }
}
