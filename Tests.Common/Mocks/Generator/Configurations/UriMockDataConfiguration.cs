namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class UriMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => type == typeof(Uri);

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => new Uri("http://example.com");
}
