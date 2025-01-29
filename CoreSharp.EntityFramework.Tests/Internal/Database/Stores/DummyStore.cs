using CoreSharp.EntityFramework.Stores.Abstracts;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Stores;

public sealed class DummyStore(DbContext dbContext)
    : StoreBase<DummyEntity, Guid>(dbContext), IDummyStore
{
}

