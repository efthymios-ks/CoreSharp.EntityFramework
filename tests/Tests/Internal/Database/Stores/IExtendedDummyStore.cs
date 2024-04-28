using CoreSharp.EntityFramework.Stores.Interfaces;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Database.Stores;

public interface IExtendedDummyStore : IExtendedStore<DummyEntity, Guid>
{
}

