using CoreSharp.Extensions;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Extensions;

/// <summary>
/// <see cref="IQueryable{T}"/> extensions.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Paginate collection on given size and return page of given number.
    /// </summary>
    public static Task<Page<TEntity>> GetPageAsync<TEntity>(this IQueryable<TEntity> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        // Validate args 
        _ = query ?? throw new ArgumentNullException(nameof(query));
        if (pageNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), $"{nameof(pageNumber)} has to be positive.");
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} has to be positive and non-zero.");

        return GetPageInternalAsync(query, pageNumber, pageSize, cancellationToken);
    }

    private static async Task<Page<TEntity>> GetPageInternalAsync<TEntity>(this IQueryable<TEntity> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        // Calculate and paginate 
        var pagedQuery = query.GetPage(pageNumber, pageSize);
        TEntity[] items;
        int totalItems;
        if (query is IAsyncEnumerable<TEntity>)
        {
            items = await pagedQuery.ToArrayAsync(cancellationToken);
            totalItems = await query.CountAsync(cancellationToken);
        }
        else
        {
            items = pagedQuery.ToArray();
            totalItems = query.Count();
        }

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Return
        return new(pageNumber, pageSize, totalItems, totalPages, items);
    }
}
