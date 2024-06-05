using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

public sealed class ExtendedDummyRepository(DbContext dbContext)
    : ExtendedRepositoryBase<DummyEntity, Guid>(dbContext), IExtendedDummyRepository
{
}
