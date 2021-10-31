using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models.Abstracts;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models
{
    public class StudentAddress : BaseAddress
    {
        //Properties
        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}
