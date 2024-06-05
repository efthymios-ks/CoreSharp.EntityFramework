using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

public sealed class DummyRepository(DbContext dbContext)
    : RepositoryBase<DummyEntity, Guid>(dbContext), IDummyRepository
{
}
