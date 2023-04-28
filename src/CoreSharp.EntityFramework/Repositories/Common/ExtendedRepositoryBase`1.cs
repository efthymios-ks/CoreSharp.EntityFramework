using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.Extensions;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Common;

public abstract class ExtendedRepositoryBase<TEntity> : RepositoryBase<TEntity>, IExtendedRepository<TEntity>
    where TEntity : class, IEntity
{
    // Constructors
    protected ExtendedRepositoryBase(DbContext dbContext)
        : base(dbContext)
    {
    }

    // Methods 
    public virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        return await Table.AddManyAsync(entities, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        return await Table.AttachManyAsync(entities);
    }

    public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        await Table.RemoveManyAsync(entities);
    }

    public virtual async Task RemoveByKeyAsync(object key, CancellationToken cancellationToken = default)
    {
        _ = key ?? throw new ArgumentNullException(nameof(key));

        await Context.Set<TEntity>()
                     .RemoveByKeyAsync(key, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(object key, CancellationToken cancellationToken = default)
    {
        _ = key ?? throw new ArgumentNullException(nameof(key));

        return await ExistsAsync(q => q.Where(e => Equals(e.Id, key)), cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation);
        var foundIds = await query.Select(e => e.Id)
                                  .Take(1)
                                  .AsNoTracking()
                                  .ToArrayAsync(cancellationToken);
        return foundIds.Length > 0;
    }

    public virtual async Task<long> CountAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation).AsNoTracking();
        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        return await ExistsAsync(entity.Id, cancellationToken)
                ? await UpdateAsync(entity, cancellationToken)
                : await AddAsync(entity, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        return await Table.AddOrAttachManyAsync(entities, cancellationToken);
    }

    public virtual async Task<TEntity> AddIfNotExistAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        var existing = await GetAsync(entity.Id, cancellationToken: cancellationToken);
        return existing ?? await AddAsync(entity, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> AddIfNotExistAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        return await Table.AddManyIfNotExistAsync(entities, cancellationToken);
    }

    public virtual async Task<TEntity> UpdateIfExistAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        if (await ExistsAsync(entity.Id, cancellationToken))
            entity = await UpdateAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateIfExistAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        return await Table.AttachManyIfExistAsync(entities, cancellationToken);
    }

    public virtual async Task<Page<TEntity>> GetPageAsync(int pageNumber, int pageSize, Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation);
        return await query.GetPageAsync(pageNumber, pageSize, cancellationToken);
    }
}
