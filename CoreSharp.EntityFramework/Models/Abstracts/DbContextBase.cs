using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    public abstract class DbContextBase : DbContext
    {
        //Constructors
        protected DbContextBase()
            => SavingChanges += SavingChangesEventHandler;

        //Events
        private void SavingChangesEventHandler(object sender, SavingChangesEventArgs e)
            => UpdateTrackableEntities();

        //Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Trackable entities
            foreach (var trackableEntity in modelBuilder.Model.GetEntityTypes(typeof(ITrackedEntity)))
            {
                var trackableEntityBuilder = modelBuilder.Entity(trackableEntity.Name);

                //DateCreatedUtc
                var dateCreatedProperty = trackableEntityBuilder
                    .Property(nameof(ITrackedEntity.DateCreatedUtc)) as PropertyBuilder<DateTime>;
                dateCreatedProperty.HasUtcConversion();

                //DateModifiedUtc
                var dateModifiedProperty = trackableEntityBuilder
                    .Property(nameof(ITrackedEntity.DateModifiedUtc)) as PropertyBuilder<DateTime?>;
                dateModifiedProperty.HasUtcConversion();
            }
        }

        private void UpdateTrackableEntities()
        {
            var trackedEntries = ChangeTracker.Entries().Where(e => e.Entity is ITrackedEntity);
            foreach (var trackedEntry in trackedEntries)
            {
                var trackedEntity = trackedEntry.Entity as ITrackedEntity;
                switch (trackedEntry.State)
                {
                    case EntityState.Added:
                        trackedEntity.DateCreatedUtc = DateTime.UtcNow;
                        trackedEntry.Property(nameof(ITrackedEntity.DateModifiedUtc)).IsModified = false;
                        break;
                    case EntityState.Modified:
                        trackedEntity.DateModifiedUtc = DateTime.UtcNow;
                        trackedEntry.Property(nameof(ITrackedEntity.DateCreatedUtc)).IsModified = false;
                        break;
                }
            }
        }
    }
}
