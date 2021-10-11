using System;

namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface ITrackableEntity
    {
        //Properties
        DateTime DateCreatedUtc { get; set; }
        DateTime? DateModifiedUtc { get; set; }
    }
}
