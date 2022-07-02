﻿using CoreSharp.EntityFramework.Entities;
using CoreSharp.EntityFramework.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.DbContexts.Common;

public abstract class TrackableDbContextBase : DbContextBase
{
    //Fields
    private TemporaryEntityChange[] _temporaryChanges;

    //Constructors
    protected TrackableDbContextBase(DbContextOptions options)
        : base(options)
    {
    }

    protected TrackableDbContextBase()
    {
    }

    //Properties
    public DbSet<EntityChange> DataChanges { get; set; }

    //Methods 
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();
        var count = base.SaveChanges(acceptAllChangesOnSuccess);
        OnAfterSaveChanges(acceptAllChangesOnSuccess);
        return count;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        var count = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await OnAfterSaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        return count;
    }

    private void OnBeforeSaveChanges()
        => CacheChangesBeforeSave();

    private void OnAfterSaveChanges(bool acceptAllChangesOnSuccess)
    {
        var changes = FinalizeAndGetChangesAfterSave();
        DataChanges.AddRange(changes);
        base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private async Task OnAfterSaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        var changes = FinalizeAndGetChangesAfterSave();
        await DataChanges.AddRangeAsync(changes, cancellationToken);
        await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void CacheChangesBeforeSave()
    {
        //Force scan for changes to be sure.
        ChangeTracker.DetectChanges();

        //Initialize with an empty array to avoid null reference exceptions.
        _temporaryChanges = Array.Empty<TemporaryEntityChange>();

        static bool EntityEntryFilter(EntityEntry entry)
        {
            if (entry.Entity is EntityChange)
                return false;
            else if (entry.Entity is not ITrackableEntity trackableEntity)
                return false;
            else if (entry.State is EntityState.Unchanged or EntityState.Detached)
                return false;

            return true;
        }

        //If no valid entries detected, return. 
        var trackableEntries = ChangeTracker.Entries().Where(EntityEntryFilter);
        if (!trackableEntries.Any())
            return;

        static bool PropertyEntryFilter(PropertyEntry entry)
        {
            var propertyName = entry.Metadata.Name;
            if (propertyName == nameof(ITrackableEntity.DateCreatedUtc))
                return false;
            else if (propertyName == nameof(ITrackableEntity.DateModifiedUtc))
                return false;

            return true;
        }

        //Scan entries 
        var temporaryChange = new HashSet<TemporaryEntityChange>();
        foreach (var trackableEntry in trackableEntries)
        {
            var change = new TemporaryEntityChange(trackableEntry);

            //Scan properties 
            var properties = trackableEntry.Properties.Where(PropertyEntryFilter);
            foreach (var property in properties)
            {
                var propertyName = property.Metadata.Name;

                //Value will be generated by the database, get the value after saving. 
                if (property.IsTemporary)
                {
                    change.TemporaryProperties.Add(property);
                    continue;
                }

                //Primary keys
                if (property.Metadata.IsPrimaryKey())
                {
                    change.Keys[propertyName] = property.CurrentValue;
                    continue;
                }

                //Properties 
                switch (trackableEntry.State)
                {
                    case EntityState.Added:
                        change.NewState[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        change.PreviousState[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified when property.IsModified:
                        change.PreviousState[propertyName] = property.OriginalValue;
                        change.NewState[propertyName] = property.CurrentValue;
                        break;
                }
            }

            temporaryChange.Add(change);
        }

        _temporaryChanges = temporaryChange.ToArray();
    }

    private EntityChange[] FinalizeAndGetChangesAfterSave()
    {
        try
        {
            //Check temporary changes with temporary propertries only. 
            var temporaryChanges = _temporaryChanges?.Where(c => c.TemporaryProperties.Count > 0);
            temporaryChanges ??= Enumerable.Empty<TemporaryEntityChange>();
            foreach (var temporaryChange in temporaryChanges)
            {
                //Get the final value of the temporary properties. 
                foreach (var property in temporaryChange.TemporaryProperties.ToArray())
                {
                    var propertyName = property.Metadata.Name;
                    var propertyValue = property.CurrentValue;

                    if (property.Metadata.IsPrimaryKey())
                        temporaryChange.Keys[propertyName] = propertyValue;
                    else
                        temporaryChange.NewState[propertyName] = propertyValue;

                    temporaryChange.TemporaryProperties.Remove(property);
                }
            }

            //Convert and return all temporary changes, regardless. 
            return _temporaryChanges.Select(c => c.ToEntityChange())
                                    .ToArray();
        }
        finally
        {
            //Always clear local temporary changes. 
            _temporaryChanges = null;
        }
    }
}
