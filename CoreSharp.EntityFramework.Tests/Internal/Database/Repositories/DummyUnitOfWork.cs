using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;

public sealed class DummyUnitOfWork(DbContext dbContext)
    : UnitOfWorkBase(dbContext), IDummyUnitOfWork
{
}
