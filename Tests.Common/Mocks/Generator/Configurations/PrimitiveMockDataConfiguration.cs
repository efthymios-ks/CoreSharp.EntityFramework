namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class PrimitiveMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
    {
        var baseType = Nullable.GetUnderlyingType(type) ?? type;

        if (baseType.IsEnum)
        {
            baseType = Enum.GetUnderlyingType(baseType);
        }

        if (baseType.IsPrimitive)
        {
            return true;
        }

        var additionalTypes = new[]
        {
            typeof(string),
            typeof(decimal),
            typeof(Guid),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(DateTimeOffset)
        };

        return Array.Exists(additionalTypes, type => type == baseType);
    }

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
    {
        var baseType = Nullable.GetUnderlyingType(type) ?? type;

        if (baseType == typeof(byte))
        {
            return (byte)mockDataGenerator.Random.Next(byte.MinValue, byte.MaxValue);
        }

        if (baseType == typeof(int))
        {
            return mockDataGenerator.Random.Next();
        }

        if (baseType == typeof(long))
        {
            return (long)mockDataGenerator.Random.Next();
        }

        if (baseType == typeof(double))
        {
            return mockDataGenerator.Random.NextDouble();
        }

        if (baseType == typeof(string))
        {
            return Guid.NewGuid().ToString();
        }

        if (baseType == typeof(bool))
        {
            return mockDataGenerator.Random.Next(0, 2) == 1;
        }

        if (baseType == typeof(DateTime))
        {
            var range = (DateTime.MaxValue - DateTime.MinValue).TotalSeconds;
            var offset = mockDataGenerator.Random.NextDouble() * range;
            return DateTime.MinValue.AddSeconds(offset);
        }

        if (baseType == typeof(DateTimeOffset))
        {
            var range = (DateTime.MaxValue - DateTime.MinValue).TotalSeconds;
            var offset = mockDataGenerator.Random.NextDouble() * range;
            return new DateTimeOffset(DateTime.MinValue.AddSeconds(offset));
        }

        if (baseType == typeof(Guid))
        {
            return Guid.NewGuid();
        }

        if (baseType == typeof(TimeSpan))
        {
            const int secondsInAday = 24 * 60 * 60;
            return TimeSpan.FromSeconds(mockDataGenerator.Random.Next(0, secondsInAday));
        }

        if (baseType == typeof(decimal))
        {
            return new decimal(mockDataGenerator.Random.NextDouble());
        }

        if (baseType.IsEnum)
        {
            var source = Enum.GetValues(baseType);
            var max = source.Length;
            return source.GetValue(mockDataGenerator.Random.Next(max))!;
        }

        throw new Exception($"Unable to generate a value for {type.FullName}");
    }
}
