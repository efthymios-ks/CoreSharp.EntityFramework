﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace CoreSharp.EntityFramework.Entities;

/// <summary>
/// For internal, temporary use only.
/// </summary>
internal sealed class TemporaryEntityChange
{
    // Constructors
    public TemporaryEntityChange(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        TableName = entry.Metadata.GetTableName()!;
        Action = entry.State.ToString();
    }

    // Properties 
    public string TableName { get; }
    public string Action { get; }
    public IDictionary<string, object?> Keys { get; } = new Dictionary<string, object?>();
    public IDictionary<string, object?> PreviousState { get; } = new Dictionary<string, object?>();
    public IDictionary<string, object?> NewState { get; } = new Dictionary<string, object?>();
    public ICollection<PropertyEntry> TemporaryProperties { get; } = [];

    // Methods
    public EntityChange ToEntityChange()
        => new()
        {
            TableName = TableName,
            Action = Action,
            Keys = SerializeDictionary(Keys),
            PreviousState = SerializeDictionary(PreviousState),
            NewState = SerializeDictionary(NewState)
        };

    private static string? SerializeDictionary(IDictionary<string, object?> dictionary)
        => dictionary.Any() ? JsonSerializer.Serialize(dictionary) : null;
}
