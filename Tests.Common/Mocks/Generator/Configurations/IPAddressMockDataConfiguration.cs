using System.Net;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class IPAddressMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => type == typeof(IPAddress);

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => new IPAddress(mockDataGenerator.Create<long>());
}
