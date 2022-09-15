using CoreSharp.EntityFramework.Entities.Common;
using System;
using System.Collections.Generic;

namespace Domain.Database.Models;

public class Teacher : EntityBase<Guid>
{
    // Properties
    public string Name { get; set; }
    public TeacherType TeacherType { get; set; }
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
