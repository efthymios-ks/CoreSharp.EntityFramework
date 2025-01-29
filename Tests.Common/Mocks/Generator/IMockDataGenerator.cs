namespace Tests.Common.Mocks.Generator;

/// <summary>
/// Defines methods for generating mock data and freezing instances of types.
/// </summary>
public interface IMockDataGenerator
{
    Random Random { get; }

    /// <inheritdoc cref="Configure(IMockDataConfiguration)" />
    void Configure<TElement>(Func<TElement> factory);

    /// <summary>
    /// Configures a factory for generating instances of a specific type.
    /// </summary>
    void Configure(IMockDataConfiguration configuration);

    /// <summary>
    /// Configures a factory for generating instances of interface or abstract types.
    /// </summary> 
    void ConfigureAbstractionFactory(Func<Type, object> factory);

    /// <inheritdoc cref="Create(Type)"/>>
    TElement Create<TElement>();

    /// <summary>
    /// Creates an instance of the specified type.
    /// </summary> 
    object Create(Type type);

    /// <inheritdoc cref="CreateMany(Type, int?)"/>>
    IEnumerable<TElement> CreateMany<TElement>(int? count = null);

    /// <summary>
    /// Creates a collection of instances of the specified type.
    /// </summary>
    IEnumerable<object> CreateMany(Type type, int? count = null);

    /// <inheritdoc cref="Freeze(Type)"/>>
    TElement Freeze<TElement>();

    /// <summary>
    /// Freezes an instance of a specific type, so that subsequent calls to <see cref="Create"/> 
    /// return the same instance.
    /// </summary>
    object Freeze(Type type);
}
