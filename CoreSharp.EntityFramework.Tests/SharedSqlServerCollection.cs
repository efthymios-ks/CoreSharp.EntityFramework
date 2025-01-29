namespace CoreSharp.EntityFramework.Tests;

[CollectionDefinition(nameof(SharedSqlServerCollection))]
public sealed class SharedSqlServerCollection : ICollectionFixture<SharedSqlServerContainer>;
