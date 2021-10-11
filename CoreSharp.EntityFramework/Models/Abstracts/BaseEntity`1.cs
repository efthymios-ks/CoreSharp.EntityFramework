using CoreSharp.EntityFramework.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace CoreSharp.EntityFramework.Models.Abstracts
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        //Fields 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        private DateTime? _dateCreatedUtc;

        //Properties 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
            => ToString();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

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
