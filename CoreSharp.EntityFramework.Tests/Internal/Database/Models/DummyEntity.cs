﻿using CoreSharp.EntityFramework.Entities.Abstracts;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Models;

public class DummyEntity : EntityBase<Guid>
{
    // Properties 
    public string? Name { get; set; }
    public virtual Guid AutoGeneratedId { get; set; }
}
