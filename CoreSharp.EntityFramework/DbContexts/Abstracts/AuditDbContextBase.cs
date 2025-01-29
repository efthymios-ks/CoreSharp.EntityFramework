﻿using CoreSharp.EntityFramework.Entities;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreSharp.EntityFramework.DbContexts.Abstracts;

public abstract class AuditDbContextBase(DbContextOptions options) : DbContext(options)
{
    private static readonly string _auditEntityDateCreatedPropertyName = nameof(IAuditEntity.DateCreatedUtc);
    private static readonly string _auditEntityDateModifiedPropertyName = nameof(IAuditEntity.DateModifiedUtc);

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

    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return ValueTask.CompletedTask;
        }

        IsDisposed = true;
        GC.SuppressFinalize(this);
        return base.DisposeAsync();
    }

    private static void ConfigureAuditEntities(ModelBuilder modelBuilder)
    {
        var auditEntities = modelBuilder.Model.FindEntityTypes(typeof(IAuditEntity));
        foreach (var auditEntity in auditEntities)
        {
            var entityTypeBuilder = modelBuilder.Entity(auditEntity.Name);

            // DateCreatedUtc
            var dateCreatedProperty = (PropertyBuilder<DateTime>)entityTypeBuilder
                .Property(nameof(IAuditEntity.DateCreatedUtc));
            dateCreatedProperty.HasUtcConversion();

            // DateModifiedUtc
            var dateModifiedProperty = (PropertyBuilder<DateTime?>)entityTypeBuilder
                .Property(nameof(IAuditEntity.DateModifiedUtc));
            dateModifiedProperty.HasUtcConversion();
        }
    }

    private TemporaryEntityChange[] OnBeforeSaveChanges()
    {
        UpdateAuditEntities();
        return GetChangesBeforeSave();
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

    private TemporaryEntityChange[] GetChangesBeforeSave()
    {
        // Force scan for changes to be sure.
        ChangeTracker.DetectChanges();

        // If no valid entries detected, return. 
        var auditEntityEntries = ChangeTracker.Entries().Where(EntityEntryFilter);
        if (!auditEntityEntries.Any())
        {
            return [];
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

        return [.. temporaryEntityChanges];

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

    private void UpdateAuditEntities()
    {
        var auditEntityEntries = ChangeTracker.Entries<IAuditEntity>();
        foreach (var auditEntityEntry in auditEntityEntries)
        {
            var auditEntity = auditEntityEntry.Entity;
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
}
