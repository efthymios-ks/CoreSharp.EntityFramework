using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;

public interface IExtendedDummyRepository : IExtendedRepository<DummyEntity, Guid>;
