using CoreSharp.EntityFramework.Models.Abstracts;
using System;
using System.Collections.Generic;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Models
{
    public class Course : EntityBase<Guid>
    {
        //Properties
        public string Name { get; set; }
        public Guid TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public virtual ICollection<CourseField> Fields { get; set; } = new HashSet<CourseField>();
    }
}
