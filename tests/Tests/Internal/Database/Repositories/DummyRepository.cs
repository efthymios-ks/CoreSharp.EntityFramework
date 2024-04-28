using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

public sealed class DummyRepository : RepositoryBase<DummyEntity, Guid>, IDummyRepository
{
    public DummyRepository(DbContext dbContext)
        : base(dbContext)
    {
    }
}
