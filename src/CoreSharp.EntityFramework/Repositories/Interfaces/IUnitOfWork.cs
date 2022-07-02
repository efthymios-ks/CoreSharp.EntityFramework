using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    //Methods 
    /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)" />
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="DbContextExtensions.RollbackAsync(DbContext, CancellationToken)" />
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
