using CoreSharp.EntityFramework.Models.Abstracts;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models.Abstracts
{
    public abstract class BaseAddress : EntityBase<Guid>
    {
        //Properties 
        public string Address { get; set; }
        public int PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
