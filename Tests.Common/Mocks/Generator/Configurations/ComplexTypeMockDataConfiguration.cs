using Tests.Common.Extensions;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class ComplexTypeMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => true;

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
    {
        var instance = ConstructComplexInstance(type, mockDataGenerator);

        foreach (var property in type
            .GetPublicOrInternalProperties()
            .Where(property => property.CanWrite))
        {
            var propertyType = property.PropertyType;
            var randomValue = mockDataGenerator.Create(propertyType);
            property.SetValue(instance, randomValue);
        }

        return instance;
    }

    private object ConstructComplexInstance(Type type, IMockDataGenerator mockDataGenerator)
    {
        var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
        if (parameterlessConstructor is not null)
        {
            return Activator.CreateInstance(type)!;
        }

        var constructor = type
            .GetConstructors(/*BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance*/)
            .Select(constructor => new
            {
                Constructor = constructor,
                Parameters = constructor.GetParameters()
            })
            .OrderByDescending(constructor => constructor.Parameters.Length)
            .FirstOrDefault();

        if (constructor is not null)
        {
            var arguments = constructor
                .Parameters
                .Select(parameter => mockDataGenerator.Create(parameter.ParameterType))
                .ToArray();

            return constructor.Constructor.Invoke(arguments);
        }

        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch
        {
            throw new Exception($"Unable to create an instance of {type.Name}");
        }
    }
}
