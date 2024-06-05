using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoreSharp.EntityFramework.Repositories.Abstracts;

/// <inheritdoc cref="IUnitOfWork"/>
public abstract class UnitOfWorkBase : IUnitOfWork
{
    // Constructors
    protected UnitOfWorkBase(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        Context = dbContext;
    }

    // Properties
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal bool IsDisposed { get; set; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected DbContext Context { get; }

    // Methods   
    public virtual Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);

    public virtual Task RollbackAsync(CancellationToken cancellationToken = default)
        => Context.RollbackAsync(cancellationToken);

    public virtual ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return ValueTask.CompletedTask;
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
        return Context.DisposeAsync();
    }
}
