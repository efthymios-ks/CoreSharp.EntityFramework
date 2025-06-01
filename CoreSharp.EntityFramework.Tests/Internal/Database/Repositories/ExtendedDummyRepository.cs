using CoreSharp.EntityFramework.Repositories.Abstracts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;

public sealed class ExtendedDummyRepository(DbContext dbContext)
    : ExtendedRepositoryBase<DummyEntity, Guid>(dbContext), IExtendedDummyRepository;
