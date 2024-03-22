using System;

namespace CoreSharp.EntityFramework.Entities.Interfaces;

public interface IAuditEntity
{
    // Properties
    DateTime DateCreatedUtc { get; set; }
    DateTime? DateModifiedUtc { get; set; }
}
