using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;

public interface IDummyRepository : IRepository<DummyEntity, Guid>
{
}
