using CoreSharp.EntityFramework.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        //Fields 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        private DateTime? _dateCreatedUtc;

        //Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        object IKeyedEntity.Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreatedUtc
        {
            get => _dateCreatedUtc ?? DateTime.UtcNow;
            set => _dateCreatedUtc = value;
        }

        [DataType(DataType.DateTime)]
        public DateTime? DateModifiedUtc { get; set; }

        //Methods 
        public override string ToString()
            => $"{Id}";
    }
}
