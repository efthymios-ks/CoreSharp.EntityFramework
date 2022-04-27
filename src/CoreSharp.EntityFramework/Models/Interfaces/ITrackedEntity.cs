using System;

namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface ITrackedEntity
    {
        //Properties
        DateTime DateCreatedUtc { get; set; }
        DateTime? DateModifiedUtc { get; set; }
    }
}
