using CoreSharp.EntityFramework.Entities.Interfaces;
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
    object IUniqueEntity.Id { get; set; } = null!;

    [Column(Order = 1)]
    public DateTime DateCreatedUtc
    {
        get => _dateCreatedUtc ?? DateTime.UtcNow;
        set => _dateCreatedUtc = value.ToUniversalTime();
    }

    [Column(Order = 2)]
    public DateTime? DateModifiedUtc
    {
        get => _dateModifiedUtc;
        set => _dateModifiedUtc = value?.ToUniversalTime();
    }

    // Methods 
    public override string? ToString()
        => ((IUniqueEntity)this).Id?.ToString();
}
