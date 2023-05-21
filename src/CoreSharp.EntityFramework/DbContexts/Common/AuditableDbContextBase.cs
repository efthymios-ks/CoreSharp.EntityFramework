using CoreSharp.EntityFramework.Entities;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.DbContexts.Common;

public abstract class AuditableDbContextBase : DbContext
{
    // Constructors
    protected AuditableDbContextBase(DbContextOptions options)
        : base(options)
    {
    }

    protected AuditableDbContextBase()
        : base()
    {
    }

    // Properties
    public DbSet<EntityChange> DataChanges { get; set; }

    // Methods 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTrackableEntities(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var temporaryEntityChanges = OnBeforeSaveChanges();
        var databaseChangeCount = base.SaveChanges(acceptAllChangesOnSuccess);
        OnAfterSaveChanges(acceptAllChangesOnSuccess, temporaryEntityChanges);
        return databaseChangeCount;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var temporaryEntityChanges = OnBeforeSaveChanges();
        var databaseChangeCount = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await OnAfterSaveChangesAsync(acceptAllChangesOnSuccess, temporaryEntityChanges, cancellationToken);
        return databaseChangeCount;
    }

    private TemporaryEntityChange[] OnBeforeSaveChanges()
    {
        UpdateTrackableEntities();
        return CacheChangesBeforeSave();
    }

    private void OnAfterSaveChanges(bool acceptAllChangesOnSuccess, TemporaryEntityChange[] temporaryEntityChanges)
    {
        var entityChanges = FinalizeAndGetChangesAfterSave(temporaryEntityChanges);
        DataChanges.AddRange(entityChanges);
        base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private async Task OnAfterSaveChangesAsync(bool acceptAllChangesOnSuccess, TemporaryEntityChange[] temporaryEntityChanges, CancellationToken cancellationToken)
    {
        var entityChanges = FinalizeAndGetChangesAfterSave(temporaryEntityChanges);
        await DataChanges.AddRangeAsync(entityChanges, cancellationToken);
        await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private static void ConfigureTrackableEntities(ModelBuilder modelBuilder)
    {
        // Trackable entities
        var trackableEntities = modelBuilder.Model.FindEntityTypes(typeof(IAuditableEntity));
        foreach (var trackableEntity in trackableEntities)
        {
            var trackedEntityBuilder = modelBuilder.Entity(trackableEntity.Name);

            // DateCreatedUtc
            var dateCreatedProperty = trackedEntityBuilder
                .Property(nameof(IAuditableEntity.DateCreatedUtc)) as PropertyBuilder<DateTime>;
            dateCreatedProperty.HasUtcConversion();

            // DateModifiedUtc
            var dateModifiedProperty = trackedEntityBuilder
                .Property(nameof(IAuditableEntity.DateModifiedUtc)) as PropertyBuilder<DateTime?>;
            dateModifiedProperty.HasUtcConversion();
        }
    }

    private void UpdateTrackableEntities()
    {
        var trackableEntries = ChangeTracker.Entries().Where(e => e.Entity is IAuditableEntity);
        foreach (var trackableEntry in trackableEntries)
        {
            var trackableEntity = trackableEntry.Entity as IAuditableEntity;

            switch (trackableEntry.State)
            {
                case EntityState.Added:
                    trackableEntity.DateCreatedUtc = DateTime.UtcNow;
                    trackableEntry.Property(nameof(IAuditableEntity.DateModifiedUtc)).IsModified = false;
                    break;
                case EntityState.Modified:
                    trackableEntity.DateModifiedUtc = DateTime.UtcNow;
                    trackableEntry.Property(nameof(IAuditableEntity.DateCreatedUtc)).IsModified = false;
                    break;
            }
        }
    }

    private TemporaryEntityChange[] CacheChangesBeforeSave()
    {
        // Force scan for changes to be sure.
        ChangeTracker.DetectChanges();

        static bool EntityEntryFilter(EntityEntry entry)
        {
            if (entry.Entity is EntityChange)
            {
                return false;
            }
            else if (entry.Entity is not IAuditableEntity)
            {
                return false;
            }
            else if (entry.State is EntityState.Unchanged or EntityState.Detached)
            {
                return false;
            }

            return true;
        }

        // If no valid entries detected, return. 
        var trackableEntries = ChangeTracker.Entries().Where(EntityEntryFilter);
        if (!trackableEntries.Any())
        {
            return Array.Empty<TemporaryEntityChange>();
        }

        static bool PropertyEntryFilter(PropertyEntry entry)
        {
            var propertyName = entry.Metadata.Name;
            if (propertyName == nameof(IAuditableEntity.DateCreatedUtc))
            {
                return false;
            }
            else if (propertyName == nameof(IAuditableEntity.DateModifiedUtc))
            {
                return false;
            }

            return true;
        }

        // Scan entries 
        var temporaryEntityChanges = new HashSet<TemporaryEntityChange>();
        foreach (var trackableEntry in trackableEntries)
        {
            var temporaryEntityChange = new TemporaryEntityChange(trackableEntry);

            // Scan properties 
            var properties = trackableEntry.Properties.Where(PropertyEntryFilter);
            foreach (var property in properties)
            {
                var propertyName = property.Metadata.Name;

                // Value will be generated by the database, get the value after saving. 
                if (property.IsTemporary)
                {
                    temporaryEntityChange.TemporaryProperties.Add(property);
                    continue;
                }

                // Primary keys
                if (property.Metadata.IsPrimaryKey())
                {
                    temporaryEntityChange.Keys[propertyName] = property.CurrentValue;
                    continue;
                }

                // Properties 
                switch (trackableEntry.State)
                {
                    case EntityState.Added:
                        temporaryEntityChange.NewState[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        temporaryEntityChange.PreviousState[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified when property.IsModified:
                        temporaryEntityChange.PreviousState[propertyName] = property.OriginalValue;
                        temporaryEntityChange.NewState[propertyName] = property.CurrentValue;
                        break;
                }
            }

            temporaryEntityChanges.Add(temporaryEntityChange);
        }

        return temporaryEntityChanges.ToArray();
    }

    private static EntityChange[] FinalizeAndGetChangesAfterSave(TemporaryEntityChange[] temporaryEntityChanges)
    {
        // Check temporary changes with temporary properties only. 
        var temporaryChanges = temporaryEntityChanges.Where(c => c.TemporaryProperties.Count > 0);
        foreach (var temporaryChange in temporaryChanges)
        {
            // Get the final value of the temporary properties. 
            foreach (var property in temporaryChange.TemporaryProperties.ToArray())
            {
                var propertyName = property.Metadata.Name;
                var propertyValue = property.CurrentValue;

                if (property.Metadata.IsPrimaryKey())
                {
                    temporaryChange.Keys[propertyName] = propertyValue;
                }
                else
                {
                    temporaryChange.NewState[propertyName] = propertyValue;
                }

                temporaryChange.TemporaryProperties.Remove(property);
            }
        }

        // Convert and return all temporary changes, regardless. 
        return temporaryEntityChanges.Select(c => c.ToEntityChange())
                                     .ToArray();
    }
}
