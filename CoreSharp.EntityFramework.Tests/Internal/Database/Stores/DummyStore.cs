using CoreSharp.EntityFramework.Stores.Abstracts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Stores;

public sealed class DummyStore(DbContext dbContext)
    : StoreBase<DummyEntity, Guid>(dbContext), IDummyStore;
