using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Database.Stores;

public sealed class DummyStore : StoreBase<DummyEntity, Guid>, IDummyStore
{
    public DummyStore(DbContext dbContext)
        : base(dbContext)
    {
    }
}

