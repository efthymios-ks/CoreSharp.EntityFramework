namespace CoreSharp.EntityFramework.Tests.Internal;

[CollectionDefinition(nameof(DummySqlServerCollection), DisableParallelization = true)]
public sealed class DummySqlServerCollection : ICollectionFixture<DummySqlServerContainer>;
