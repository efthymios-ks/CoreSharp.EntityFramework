using Microsoft.AspNetCore.Http;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class PathStringMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => type == typeof(PathString);

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => PathString.Empty;
}
