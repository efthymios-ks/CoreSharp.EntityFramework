using CoreSharp.EntityFramework.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        //Fields 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        private DateTime? _dateCreatedUtc;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        private DateTime? _dateModifiedUtc;

        //Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        private string DebuggerDisplay => ToString();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        object IKeyedEntity.Id { get; set; }

        [DataType(DataType.DateTime)]
        [Newtonsoft.Json.JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime DateCreatedUtc
        {
            get => _dateCreatedUtc ?? DateTime.UtcNow;
            set => _dateCreatedUtc = TimeZoneInfo.ConvertTimeToUtc(value);
        }

        [DataType(DataType.DateTime)]
        [Newtonsoft.Json.JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime? DateModifiedUtc
        {
            get => _dateModifiedUtc;
            set
            {
                if (value is null)
                    _dateModifiedUtc = null;
                else
                    _dateModifiedUtc = TimeZoneInfo.ConvertTimeToUtc(value.Value);
            }
        }

        //Methods 
        public override string ToString()
            => $"{Id}";
    }
}
