using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Repositories.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    // Methods 
    /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)" />
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="DbContextExtensions.RollbackAsync(DbContext, CancellationToken)" />
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
