using CoreSharp.EntityFramework.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using JsonNet = Newtonsoft.Json;
using JsonNetConverters = CoreSharp.Json.JsonNet.JsonConverters;
using TextJson = System.Text.Json;
using TextJsonConverters = CoreSharp.Json.TextJson.JsonConverters;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        //Fields 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [TextJson.Serialization.JsonIgnore]
        [JsonNet.JsonIgnore]
        private DateTime? _dateCreatedUtc;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [TextJson.Serialization.JsonIgnore]
        [JsonNet.JsonIgnore]
        private DateTime? _dateModifiedUtc;

        //Properties 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [TextJson.Serialization.JsonIgnore]
        [JsonNet.JsonIgnore]
        private string DebuggerDisplay
            => ToString();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id
        {
            get => (this as IUniqueEntity).Id is TKey key ? key : default;
            set => (this as IUniqueEntity).Id = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotMapped]
        [TextJson.Serialization.JsonIgnore]
        [JsonNet.JsonIgnore]
        object IUniqueEntity.Id { get; set; } = default(TKey);

        [DataType(DataType.DateTime)]
        [JsonNet.JsonConverter(typeof(JsonNetConverters.UtcDateTimeJsonConverter))]
        [TextJson.Serialization.JsonConverter(typeof(TextJsonConverters.UtcDateTimeJsonConverter))]
        public DateTime DateCreatedUtc
        {
            get => _dateCreatedUtc ?? DateTime.UtcNow;
            set => _dateCreatedUtc = TimeZoneInfo.ConvertTimeToUtc(value);
        }

        [DataType(DataType.DateTime)]
        [JsonNet.JsonConverter(typeof(JsonNetConverters.UtcDateTimeJsonConverter))]
        [TextJson.Serialization.JsonConverter(typeof(TextJsonConverters.UtcDateTimeJsonConverter))]
        public DateTime? DateModifiedUtc
        {
            get => _dateModifiedUtc;
            set => _dateModifiedUtc = value is null ? null : TimeZoneInfo.ConvertTimeToUtc(value.Value);
        }

        //Methods 
        public override string ToString()
            => $"{Id}";
    }
}
