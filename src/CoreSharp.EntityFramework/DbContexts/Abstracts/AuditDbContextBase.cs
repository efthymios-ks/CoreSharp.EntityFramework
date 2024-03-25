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

namespace CoreSharp.EntityFramework.DbContexts.Abstracts;

public abstract class AuditDbContextBase : DbContext
{
    private static readonly string _auditEntityDateCreatedPropertyName = nameof(IAuditEntity.DateCreatedUtc);
    private static readonly string _auditEntityDateModifiedPropertyName = nameof(IAuditEntity.DateModifiedUtc);

    // Constructors
    protected AuditDbContextBase(DbContextOptions options)
        : base(options)
    {
    }

    // Properties
    internal bool IsDisposed { get; set; }
    public DbSet<EntityChange> DataChanges { get; set; }

    // Methods 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureAuditEntities(modelBuilder);
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

    private static void ConfigureAuditEntities(ModelBuilder modelBuilder)
    {
        var auditEntities = modelBuilder.Model.FindEntityTypes(typeof(IAuditEntity));
        foreach (var auditEntity in auditEntities)
        {
            var entityTypeBuilder = modelBuilder.Entity(auditEntity.Name);

            // DateCreatedUtc
            var dateCreatedProperty = entityTypeBuilder
                .Property(nameof(IAuditEntity.DateCreatedUtc)) as PropertyBuilder<DateTime>;
            dateCreatedProperty.HasUtcConversion();

            // DateModifiedUtc
            var dateModifiedProperty = entityTypeBuilder
                .Property(nameof(IAuditEntity.DateModifiedUtc)) as PropertyBuilder<DateTime?>;
            dateModifiedProperty.HasUtcConversion();
        }
    }

    private TemporaryEntityChange[] OnBeforeSaveChanges()
    {
        UpdateAuditEntities();
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

    private void UpdateAuditEntities()
    {
        var auditEntityEntries = ChangeTracker.Entries().Where(e => e.Entity is IAuditEntity);
        foreach (var auditEntityEntry in auditEntityEntries)
        {
            var auditEntity = auditEntityEntry.Entity as IAuditEntity;
            switch (auditEntityEntry.State)
            {
                case EntityState.Added:
                    auditEntity.DateCreatedUtc = DateTime.UtcNow;
                    auditEntityEntry.Property(_auditEntityDateModifiedPropertyName).IsModified = false;
                    break;
                case EntityState.Modified:
                    auditEntity.DateModifiedUtc = DateTime.UtcNow;
                    auditEntityEntry.Property(_auditEntityDateCreatedPropertyName).IsModified = false;
                    break;
            }
        }
    }

    private TemporaryEntityChange[] CacheChangesBeforeSave()
    {
        // Force scan for changes to be sure.
        ChangeTracker.DetectChanges();

        // If no valid entries detected, return. 
        var auditEntityEntries = ChangeTracker.Entries().Where(EntityEntryFilter);
        if (!auditEntityEntries.Any())
        {
            return Array.Empty<TemporaryEntityChange>();
        }

        // Scan entries 
        var temporaryEntityChanges = new HashSet<TemporaryEntityChange>();
        foreach (var auditEntityEntry in auditEntityEntries)
        {
            var temporaryEntityChange = new TemporaryEntityChange(auditEntityEntry);

            // Scan properties 
            var properties = auditEntityEntry.Properties.Where(PropertyEntryFilter);
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
                switch (auditEntityEntry.State)
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

        static bool EntityEntryFilter(EntityEntry entry)
        {
            if (entry.Entity is EntityChange)
            {
                return false;
            }
            else if (entry.Entity is not IAuditEntity)
            {
                return false;
            }
            else if (entry.State is EntityState.Unchanged or EntityState.Detached)
            {
                return false;
            }

            return true;
        }

        static bool PropertyEntryFilter(PropertyEntry entry)
        {
            var propertyName = entry.Metadata.Name;
            if (propertyName == _auditEntityDateCreatedPropertyName)
            {
                return false;
            }
            else if (propertyName == _auditEntityDateModifiedPropertyName)
            {
                return false;
            }

            return true;
        }
    }

    private static EntityChange[] FinalizeAndGetChangesAfterSave(TemporaryEntityChange[] temporaryEntityChanges)
    {
        // Check temporary changes with temporary properties only. 
        var temporaryChanges = temporaryEntityChanges.Where(change => change.TemporaryProperties.Count > 0);
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
        return temporaryEntityChanges
            .Select(change => change.ToEntityChange())
            .ToArray();
    }

    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return ValueTask.CompletedTask;
        }

        IsDisposed = true;
        return base.DisposeAsync();
    }
}
