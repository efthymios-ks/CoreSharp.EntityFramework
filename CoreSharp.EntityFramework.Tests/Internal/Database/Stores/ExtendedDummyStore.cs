using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Database.Stores;

public sealed class ExtendedDummyStore(DbContext dbContext)
    : ExtendedStoreBase<DummyEntity, Guid>(dbContext), IExtendedDummyStore
{
}

