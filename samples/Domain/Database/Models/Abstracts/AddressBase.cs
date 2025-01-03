﻿using CoreSharp.EntityFramework.Entities.Abstracts;

namespace Domain.Database.Models.Abstracts;

public abstract class AddressBase : EntityBase<Guid>
{
    // Properties 
    public string? Address { get; set; }
    public int PostCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}
