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

        private void UpdateTrackableEntities()
        {
            var trackableEntries = ChangeTracker.Entries().Where(e => e.Entity is ITrackableEntity);
            foreach (var trackableEntry in trackableEntries)
            {
                var trackableEntity = trackableEntry.Entity as ITrackableEntity;
                switch (trackableEntry.State)
                {
                    case EntityState.Added:
                        trackableEntity.DateCreatedUtc = DateTime.UtcNow;
                        trackableEntry.Property(nameof(ITrackableEntity.DateModifiedUtc)).IsModified = false;
                        break;
                    case EntityState.Modified:
                        trackableEntity.DateModifiedUtc = DateTime.UtcNow;
                        trackableEntry.Property(nameof(ITrackableEntity.DateCreatedUtc)).IsModified = false;
                        break;
                }
            }
        }
    }
}
