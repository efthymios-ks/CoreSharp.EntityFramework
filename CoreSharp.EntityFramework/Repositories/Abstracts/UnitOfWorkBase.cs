using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
        public virtual async Task CommitAsync()
            => await Context?.SaveChangesAsync();

        public virtual async Task RollbackAsync()
        {
            var entities = Context?.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged);
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
                        await entity.ReloadAsync();
                        entity.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public virtual void Dispose()
        {
            Context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
