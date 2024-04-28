using CoreSharp.EntityFramework.Repositories.Interfaces;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

public interface IDummyRepository : IRepository<DummyEntity, Guid>
{
}
