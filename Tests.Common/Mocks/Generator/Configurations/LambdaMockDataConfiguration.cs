namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class LambdaMockDataConfiguration(Type targetType, Func<object> valueFactory) : IMockDataConfiguration
{
    private readonly Type _targetType = targetType;
    private readonly Func<object> _valueFactory = valueFactory;

    public bool CanCreate(Type type)
        => _targetType == type;

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => _valueFactory();
}
