﻿using CoreSharp.EntityFramework.Models.Abstracts;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Abstracts
{
    internal abstract class BaseAddress : BaseEntity<Guid>
    {
        //Properties 
        public string Address { get; set; }
        public int PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
