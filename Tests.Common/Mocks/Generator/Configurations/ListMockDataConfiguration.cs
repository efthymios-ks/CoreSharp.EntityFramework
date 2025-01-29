using System.Collections;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class ListMockDataConfiguration : CollectionMockDataConfigurationBase
{
    public override bool CanCreate(Type type)
         => type.IsGenericType
            && type.GetGenericTypeDefinition() is Type genericType
            && (genericType == typeof(IEnumerable<>)
                || genericType == typeof(ICollection<>)
                || genericType == typeof(IReadOnlyCollection<>)
                || genericType == typeof(IList<>));

    public override object Create(Type type, IMockDataGenerator mockDataGenerator)
    {
        var elementType = type.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType)!;

        var length = mockDataGenerator.Random.Next(CollectionMin, CollectionMax);
        for (var i = 0; i < length; i++)
        {
            list.Add(mockDataGenerator.Create(elementType));
        }

        return list;
    }
}
