using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        //Constructors
        protected UnitOfWorkBase(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Properties
        protected DbContext Context { get; }

        //Methods   
        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
            => await Context?.SaveChangesAsync(cancellationToken);

        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            var entities = Context?
                            .ChangeTracker
                            .Entries()
                            .Where(e => e.State != EntityState.Unchanged);
            if (entities is null)
                return;

            foreach (var entity in entities)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        await entity.ReloadAsync(cancellationToken);
                        entity.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            Context?.Dispose();
        }
    }
}
