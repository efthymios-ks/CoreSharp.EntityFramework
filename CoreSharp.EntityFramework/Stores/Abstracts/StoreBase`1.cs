﻿using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Stores.Abstracts;

public abstract class StoreBase<TEntity, TKey>(DbContext dbContext)
    : RepositoryBase<TEntity, TKey>(dbContext), IStore<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{

    // Methods 
    public override async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var createdEntity = await base.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return createdEntity;
    }

    public override async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var updatedEntity = await base.UpdateAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return updatedEntity;
    }

    public override async Task RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await base.RemoveAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
