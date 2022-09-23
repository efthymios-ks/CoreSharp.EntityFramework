using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.DbContexts.Common;

public abstract class DbContextBase : DbContext
{
    // Constructors
    protected DbContextBase(DbContextOptions options)
        : base(options)
    {
    }

    protected DbContextBase()
    {
    }

    // Methods 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTrackableEntities(modelBuilder);
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
        => UpdateTrackableEntities();

    private static void ConfigureTrackableEntities(ModelBuilder modelBuilder)
    {
        // Trackable entities
        var trackableEntities = modelBuilder.Model.FindEntityTypes(typeof(ITrackableEntity));
        foreach (var trackableEntity in trackableEntities)
        {
            var trackedEntityBuilder = modelBuilder.Entity(trackableEntity.Name);

            // DateCreatedUtc
            var dateCreatedProperty = trackedEntityBuilder
                .Property(nameof(ITrackableEntity.DateCreatedUtc)) as PropertyBuilder<DateTime>;
            dateCreatedProperty.HasUtcConversion();

            // DateModifiedUtc
            var dateModifiedProperty = trackedEntityBuilder
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
