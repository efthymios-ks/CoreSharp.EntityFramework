using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
    protected DbContext Context { get; }

    // Methods   
    public virtual Task CommitAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);

    public virtual Task RollbackAsync(CancellationToken cancellationToken = default)
        => Context.RollbackAsync(cancellationToken);

    public virtual ValueTask DisposeAsync()
        => Context.DisposeAsync();
}
