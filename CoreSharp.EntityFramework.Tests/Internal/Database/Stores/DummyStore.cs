using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Database.Stores;

public sealed class DummyStore(DbContext dbContext)
    : StoreBase<DummyEntity, Guid>(dbContext), IDummyStore
{
}

