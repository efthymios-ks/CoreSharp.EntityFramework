using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Database.Stores;

public sealed class ExtendedDummyStore : ExtendedStoreBase<DummyEntity, Guid>, IExtendedDummyStore
{
    public ExtendedDummyStore(DbContext dbContext)
        : base(dbContext)
    {
    }
}

