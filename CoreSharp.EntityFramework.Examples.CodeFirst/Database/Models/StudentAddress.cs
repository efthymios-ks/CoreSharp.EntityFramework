using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Abstracts;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models
{
    internal class StudentAddress : BaseAddress
    {
        //Properties
        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}
