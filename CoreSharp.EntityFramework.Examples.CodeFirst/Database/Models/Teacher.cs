using CoreSharp.EntityFramework.Models.Abstracts;
using System;
using System.Collections.Generic;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models
{
    internal class Teacher : BaseEntity<Guid>
    {
        //Properties
        public string Name { get; set; }
        public TeacherType TeacherType { get; set; }
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
