using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonNet = Newtonsoft.Json;
using JsonNetConverters = CoreSharp.Json.JsonNet.JsonConverters;
using TextJson = System.Text.Json;
using TextJsonConverters = CoreSharp.Json.TextJson.JsonConverters;

namespace CoreSharp.EntityFramework.Entities;

[Table("__EFDataHistory")]
public sealed class EntityChange
{
    // Fields
    private DateTime _dateCreated = DateTime.UtcNow;

    // Properties
    [Key]
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column(Order = 1)]
    [JsonNet.JsonConverter(typeof(JsonNetConverters.UtcDateTimeJsonConverter))]
    [TextJson.Serialization.JsonConverter(typeof(TextJsonConverters.UtcDateTimeJsonConverter))]
    public DateTime DateCreatedUtc
    {
        get => _dateCreated;
        set => _dateCreated = value.ToUniversalTime();
    }

    [Required]
    [Column(Order = 2)]
    public string TableName { get; set; } = string.Empty;

    [Required]
    [Column(Order = 3)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [Column(Order = 4)]
    public string? Keys { get; set; }

    [Column(Order = 5)]
    public string? PreviousState { get; set; }

    [Column(Order = 6)]
    public string? NewState { get; set; }
}
