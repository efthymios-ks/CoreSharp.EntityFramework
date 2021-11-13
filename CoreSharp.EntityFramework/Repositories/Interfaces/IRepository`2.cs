﻿using CoreSharp.EntityFramework.Models.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces
{
    /// <inheritdoc />
    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
        where TEntity : class, IEntity<TKey>
    {
        //Methods 
        /// <inheritdoc cref="IRepository{TEntity}.GetAsync(object, Func{IQueryable{TEntity}, IQueryable{TEntity}}, CancellationToken)"/>
        Task<TEntity> GetAsync(
            TKey key,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default);
    }
}
