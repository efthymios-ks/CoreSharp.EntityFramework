using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories.Abstracts;

public sealed class DummyUnitOfWork(DbContext dbContext)
    : UnitOfWorkBase(dbContext), IDummyUnitOfWork
{
}
