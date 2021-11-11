using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    public abstract class DbContextBase : DbContext
    {
        //Methods
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Trackable entities
            foreach (var trackableEntity in modelBuilder.Model.GetEntityTypes(typeof(ITrackableEntity)))
            {
                var trackableEntityBuilder = modelBuilder.Entity(trackableEntity.Name);

                //DateCreatedUtc
                var dateCreatedProperty = trackableEntityBuilder
                    .Property(nameof(ITrackableEntity.DateCreatedUtc)) as PropertyBuilder<DateTime>;
                dateCreatedProperty.HasUtcConversion();

                //DateModifiedUtc
                var dateModifiedProperty = trackableEntityBuilder
                    .Property(nameof(ITrackableEntity.DateModifiedUtc)) as PropertyBuilder<DateTime?>;
                dateModifiedProperty.HasUtcConversion();
            }
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
            var trackableEntries = ChangeTracker.Entries().Where(e => e.Entity is ITrackableEntity);
            foreach (var entry in trackableEntries)
            {
                var trackableEntity = entry.Entity as ITrackableEntity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        trackableEntity.DateCreatedUtc = DateTime.UtcNow;
                        entry.Property(nameof(ITrackableEntity.DateModifiedUtc)).IsModified = false;
                        break;
                    case EntityState.Modified:
                        trackableEntity.DateModifiedUtc = DateTime.UtcNow;
                        entry.Property(nameof(ITrackableEntity.DateCreatedUtc)).IsModified = false;
                        break;
                }
            }
        }
    }
}
