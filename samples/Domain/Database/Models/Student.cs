﻿using CoreSharp.EntityFramework.Entities.Abstracts;
using System;
using System.Collections.Generic;

namespace Domain.Database.Models;

public class Student : EntityBase<Guid>
{
    // Properties
    public string Name { get; set; }
    public Guid AddressId { get; set; }
    public virtual StudentAddress Address { get; set; }
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
