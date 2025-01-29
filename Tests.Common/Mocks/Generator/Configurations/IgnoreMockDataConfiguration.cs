namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class IgnoreMockDataConfiguration(Func<Type, bool> shouldIgnoreFunc) : IMockDataConfiguration
{
    public IgnoreMockDataConfiguration(params Type[] typesToIgnore)
        : this(type => Array.Exists(typesToIgnore, typeToIgnore => typeToIgnore == type))
    {
    }

    private readonly Func<Type, bool> _shouldIgnoreFunc = shouldIgnoreFunc;

    public bool CanCreate(Type type)
        => _shouldIgnoreFunc(type);

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => null!;
}
