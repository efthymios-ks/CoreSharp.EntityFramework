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
    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        => await Context.RollbackAsync(cancellationToken);

    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Context.DisposeAsync();
    }
}
