using NSubstitute;
using System.Reflection;
using Tests.Common.Extensions;
using Tests.Common.Mocks.Generator;
using Xunit.Abstractions;

namespace Tests.Common;

public abstract class TestsBase
{
    private readonly MockDataGenerator _mockDataGenerator = null!;
    private readonly ITestOutputHelper? _testOutputHelper;

    protected TestsBase(ITestOutputHelper testOutputHelper)
        : this()
        => _testOutputHelper = testOutputHelper;

    protected TestsBase()
    {
        _mockDataGenerator = new MockDataGenerator();
        _mockDataGenerator.ConfigureAbstractionFactory(MockWithNSubstitute);

        ConfigureMockDataGenerator(_mockDataGenerator);
    }

    protected virtual void ConfigureMockDataGenerator(IMockDataGenerator mockDataGenerator)
    {
    }

    protected TElement MockCreate<TElement>()
        => _mockDataGenerator.Create<TElement>();

    protected TElement[] MockCreateMany<TElement>()
        => _mockDataGenerator
            .CreateMany<TElement>()
            .ToArray();

    protected TElement[] MockCreateMany<TElement>(int count)
        => _mockDataGenerator
            .CreateMany<TElement>(count)
            .ToArray();

    protected TElement MockFreeze<TElement>()
        => _mockDataGenerator.Freeze<TElement>();

    private object MockWithNSubstitute(Type type)
    {
        var instance = Substitute.For([type], []);

        var properties = type.GetPublicOrInternalProperties()
            .Concat(type
                .GetInterfaces()
                .SelectMany(type => type.GetPublicOrInternalProperties()))
            .DistinctBy(property => (property.DeclaringType, property.PropertyType, property.Name))
            .Where(property => property.IsMockable())
            .ToArray();

        PropertyInfo currentProperty = null!;
        foreach (var property in properties)
        {
            currentProperty = property;

            var propertyGetter = property.GetGetMethod() ?? property.GetGetMethod(nonPublic: true)!;
            var value = _mockDataGenerator.Create(property.PropertyType);
            if (value is null)
            {
                continue;
            }

            try
            {
                propertyGetter.Invoke(instance, null).Returns(value);
            }
            catch
            {
                _testOutputHelper?.WriteLine($"Could not to mock data for type='{type}', propertyName='{currentProperty.Name}', propertyType='{currentProperty.PropertyType}'");

                throw;
            }
        }

        return instance;
    }
}
