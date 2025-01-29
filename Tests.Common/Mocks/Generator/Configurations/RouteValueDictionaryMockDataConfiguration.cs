using Microsoft.AspNetCore.Routing;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class RouteValueDictionaryMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => type == typeof(RouteValueDictionary);

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => new RouteValueDictionary();
}
