﻿using CoreSharp.EntityFramework.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using JsonNet = Newtonsoft.Json;
using TextJson = System.Text.Json;

namespace CoreSharp.EntityFramework.Entities.Abstracts;

public abstract class EntityBase : IEntity
{
    // Fields 
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [TextJson.Serialization.JsonIgnore]
    [JsonNet.JsonIgnore]
    private DateTime? _dateCreatedUtc;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [TextJson.Serialization.JsonIgnore]
    [JsonNet.JsonIgnore]
    private DateTime? _dateModifiedUtc;

    // Properties 
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [NotMapped]
    [TextJson.Serialization.JsonIgnore]
    [JsonNet.JsonIgnore]
    object IUniqueEntity.Id { get; set; }

    [Column(Order = 1)]
    public DateTime DateCreatedUtc
    {
        get => _dateCreatedUtc ?? DateTime.UtcNow;
        set => _dateCreatedUtc = SetDateTimeKindToUtc(value);
    }

    [Column(Order = 2)]
    public DateTime? DateModifiedUtc
    {
        get => _dateModifiedUtc;
        set => _dateModifiedUtc = value is null ? null : SetDateTimeKindToUtc(value.Value);
    }

    // Methods 
    public override string ToString()
        => $"{(this as IUniqueEntity).Id}";

    /// <summary>
    /// Avoid <see cref="DateTime.SpecifyKind(DateTime, DateTimeKind)"/> which converts (and ruins) the value.
    /// Use <see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime)"/> which only sets the <see cref="DateTime.Kind"/>.
    /// </summary>
    private static DateTime SetDateTimeKindToUtc(DateTime dateTime)
        => TimeZoneInfo.ConvertTimeToUtc(dateTime);
}
