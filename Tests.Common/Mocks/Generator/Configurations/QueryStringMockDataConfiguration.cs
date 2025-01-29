using Microsoft.AspNetCore.Http;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class QueryStringMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => type == typeof(QueryString);

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => QueryString.Empty;
}
