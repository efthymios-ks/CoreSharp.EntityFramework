using CoreSharp.EntityFramework.Models.Abstracts;
using System;
using System.Collections.Generic;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models
{
    internal class Student : BaseEntity<Guid>
    {
        //Properties
        public string Name { get; set; }
        public Guid AddressId { get; set; }
        public virtual StudentAddress Address { get; set; }
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
