﻿using System;

namespace CoreSharp.EntityFramework.Entities.Interfaces;

public interface ITrackableEntity
{
    //Properties
    DateTime DateCreatedUtc { get; set; }
    DateTime? DateModifiedUtc { get; set; }
}
