using System;

namespace CoreSharp.EntityFramework.Entities.Interfaces;

public interface IAuditableEntity
{
    // Properties
    DateTime DateCreatedUtc { get; set; }
    DateTime? DateModifiedUtc { get; set; }
}
