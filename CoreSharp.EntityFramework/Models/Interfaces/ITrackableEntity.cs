using System;

namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface ITrackableEntity
    {
        //TODO: Add automatic map to Utc. Currently not working. 
        //Properties
        DateTime DateCreatedUtc { get; set; }
        DateTime? DateModifiedUtc { get; set; }
    }
}
