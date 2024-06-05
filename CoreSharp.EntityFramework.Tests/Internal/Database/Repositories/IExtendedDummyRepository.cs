using CoreSharp.EntityFramework.Repositories.Interfaces;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

public interface IExtendedDummyRepository : IExtendedRepository<DummyEntity, Guid>
{
}
