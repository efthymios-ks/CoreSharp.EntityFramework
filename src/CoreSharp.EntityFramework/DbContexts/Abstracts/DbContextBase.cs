using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.DbContexts.Abstracts
{
    public abstract class DbContextBase : DbContext
    {
        //Constructors
        protected DbContextBase(DbContextOptions options)
            : base(options)
        {
        }

        protected DbContextBase()
        {
        }

        //Methods 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Trackable entities
            var trackedEntities = modelBuilder.Model.GetEntityTypes(typeof(ITrackableEntity));
            foreach (var trackedEntity in trackedEntities)
            {
                var trackedEntityBuilder = modelBuilder.Entity(trackedEntity.Name);

                //DateCreatedUtc
                var dateCreatedProperty = trackedEntityBuilder
                    .Property(nameof(ITrackableEntity.DateCreatedUtc)) as PropertyBuilder<DateTime>;
                dateCreatedProperty.HasUtcConversion();

                //DateModifiedUtc
                var dateModifiedProperty = trackedEntityBuilder
                    .Property(nameof(ITrackableEntity.DateModifiedUtc)) as PropertyBuilder<DateTime?>;
                dateModifiedProperty.HasUtcConversion();
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaveChanges();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaveChanges()
            => UpdateTrackableEntitiesDates();

        private void UpdateTrackableEntitiesDates()
        {
            var trackableEntries = ChangeTracker.Entries()
                                                .Where(e => e.Entity is ITrackableEntity);
            foreach (var trackableEntry in trackableEntries)
            {
                var trackedEntity = trackableEntry.Entity as ITrackableEntity;

                switch (trackableEntry.State)
                {
                    case EntityState.Added:
                        trackedEntity.DateCreatedUtc = DateTime.UtcNow;
                        trackableEntry.Property(nameof(ITrackableEntity.DateModifiedUtc)).IsModified = false;
                        break;
                    case EntityState.Modified:
                        trackedEntity.DateModifiedUtc = DateTime.UtcNow;
                        trackableEntry.Property(nameof(ITrackableEntity.DateCreatedUtc)).IsModified = false;
                        break;
                }
            }
        }
    }
}
