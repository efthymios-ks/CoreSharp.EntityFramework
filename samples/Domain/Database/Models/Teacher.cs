using CoreSharp.EntityFramework.Models.Abstracts;
using System;
using System.Collections.Generic;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Models
{
    public class Teacher : EntityBase<Guid>
    {
        //Properties
        public string Name { get; set; }
        public TeacherType TeacherType { get; set; }
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
