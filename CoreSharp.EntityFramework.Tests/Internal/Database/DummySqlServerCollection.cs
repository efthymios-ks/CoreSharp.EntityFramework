namespace CoreSharp.EntityFramework.Tests.Internal.Database;

[CollectionDefinition(nameof(DummySqlServerCollection), DisableParallelization = true)]
public sealed class DummySqlServerCollection : ICollectionFixture<DummySqlServerContainer>;
