using CoreSharp.EntityFramework.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    public abstract class BaseDbContext : DbContext
    {
        //Methods
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSave()
        {
            UpdateTrackableEntities();
        }

        private void UpdateTrackableEntities()
        {
            var utcNow = DateTime.UtcNow;
            var trackableEntities = ChangeTracker.Entries().Where(e => e is ITrackableEntity);
            foreach (var entity in trackableEntities)
            {
                var trackableEntity = entity as ITrackableEntity;
                switch (entity.State)
                {
                    case EntityState.Added:
                        trackableEntity.DateCreatedUtc = utcNow;
                        entity.Property(nameof(ITrackableEntity.DateModifiedUtc)).IsModified = false;
                        break;
                    case EntityState.Modified:
                        trackableEntity.DateModifiedUtc = utcNow;
                        entity.Property(nameof(ITrackableEntity.DateCreatedUtc)).IsModified = false;
                        break;
                }
            }
        }
    }
}
