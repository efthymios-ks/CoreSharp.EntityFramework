using System.Diagnostics;
using Tests.Common.Mocks.Generator.Configurations;

namespace Tests.Common.Mocks.Generator;

/// <inheritdoc />
internal sealed class MockDataGenerator : IMockDataGenerator
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<Type, object> _frozenInstances = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Func<Type, object> _abstractionFactory = _ => null!;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IList<IMockDataConfiguration> _mockDataConfigurations;

    private readonly ThreadLocal<HashSet<Type>> _processingTypes = new(() => []);

    public MockDataGenerator()
    {
        // Order matters
        _mockDataConfigurations =
        [ 
            // Ignore
            new IgnoreMockDataConfiguration(
                typeof(System.Net.Http.Headers.HttpContentHeaders),
                typeof(System.Security.Cryptography.X509Certificates.X509Certificate2)
            ),

            // Specific configurations
            new IPAddressMockDataConfiguration(),
            new PathStringMockDataConfiguration(),
            new QueryStringMockDataConfiguration(),
            new RouteValueDictionaryMockDataConfiguration(),
            new UriMockDataConfiguration(),

            // General configurations
            new ArrayMockDataConfiguration(),
            new ListMockDataConfiguration(),
            new DictionaryMockDataConfiguration(),
            new DelegateMockDataConfiguration(),
            new InterfaceOrAbstractMockDataConfiguration(type => _abstractionFactory(type)),
            new PrimitiveMockDataConfiguration(),
            new ComplexTypeMockDataConfiguration()
        ];
    }

    public Random Random { get; } = Random.Shared;

    public void Configure<TElement>(Func<TElement> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        Configure(new LambdaMockDataConfiguration(typeof(TElement), () => factory()!));
    }

    public void Configure(IMockDataConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _mockDataConfigurations.Insert(0, configuration);
    }

    public void ConfigureAbstractionFactory(Func<Type, object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _abstractionFactory = type => factory(type);
    }

    public TElement Create<TElement>()
        => (TElement)Create(typeof(TElement))!;

    public object Create(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (_frozenInstances.TryGetValue(type, out var frozenInstance))
        {
            return frozenInstance;
        }

        var processingTypes = _processingTypes.Value!;
        if (processingTypes.Contains(type))
        {
            return null!;
        }

        processingTypes.Add(type);

        try
        {
            foreach (var configuration in _mockDataConfigurations)
            {
                if (configuration.CanCreate(type))
                {
                    return configuration.Create(type, this);
                }
            }

            throw new Exception($"Unable to create an instance of {type.FullName}");
        }
        finally
        {
            processingTypes.Remove(type);
        }
    }

    public IEnumerable<TElement> CreateMany<TElement>(int? count = null)
        => CreateMany(typeof(TElement), count)
            .Cast<TElement>()
            .ToArray();

    public IEnumerable<object> CreateMany(Type type, int? count = null)
    {
        count ??= Random.Next(
            CollectionMockDataConfigurationBase.CollectionMin,
            CollectionMockDataConfigurationBase.CollectionMax
        );

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count.Value);

        return Enumerable
            .Range(0, count.Value)
            .Select(_ => Create(type))
            .ToArray();
    }

    public TElement Freeze<TElement>()
        => (TElement)Freeze(typeof(TElement))!;

    public object Freeze(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return _frozenInstances[type] = Create(type);
    }
}
