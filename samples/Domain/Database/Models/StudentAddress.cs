using CoreSharp.EntityFramework.Samples.Domain.Database.Models.Abstracts;
using System;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Models
{
    public class StudentAddress : AddressBase
    {
        //Properties
        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}
