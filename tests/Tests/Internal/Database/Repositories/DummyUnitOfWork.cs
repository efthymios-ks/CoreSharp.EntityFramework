using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories.Abstracts;

public sealed class DummyUnitOfWork : UnitOfWorkBase, IDummyUnitOfWork
{
    public DummyUnitOfWork(DbContext dbContext)
        : base(dbContext)
    {
    }
}