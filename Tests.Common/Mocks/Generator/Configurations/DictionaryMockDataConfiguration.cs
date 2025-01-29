using System.Collections;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class DictionaryMockDataConfiguration : CollectionMockDataConfigurationBase
{
    public override bool CanCreate(Type type)
        => type.IsGenericType
            && type.GetGenericTypeDefinition() is Type genericType
            && (genericType == typeof(IDictionary<,>)
                || genericType == typeof(IReadOnlyDictionary<,>));

    public override object Create(Type type, IMockDataGenerator mockDataGenerator)
    {
        var genericArguments = type.GetGenericArguments();
        var keyType = genericArguments[0];
        var valueType = genericArguments[1];
        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dictionaryInstance = (IDictionary)Activator.CreateInstance(dictionaryType)!;

        var length = mockDataGenerator.Random.Next(CollectionMin, CollectionMax);
        for (var i = 0; i < length; i++)
        {
            var key = mockDataGenerator.Create(keyType)!;
            var value = mockDataGenerator.Create(valueType);
            dictionaryInstance.Add(key, value);
        }

        return dictionaryInstance;
    }
}
