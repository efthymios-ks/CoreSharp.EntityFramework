﻿using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

public sealed class ExtendedDummyRepository : ExtendedRepositoryBase<DummyEntity, Guid>, IExtendedDummyRepository
{
    public ExtendedDummyRepository(DbContext dbContext)
        : base(dbContext)
    {
    }
}