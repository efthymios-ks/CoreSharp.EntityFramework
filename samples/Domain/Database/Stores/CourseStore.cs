using CoreSharp.EntityFramework.Stores.Abstracts;
using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;
using System;

namespace Domain.Database.Stores;

public class CourseStore : StoreBase<Course, Guid>, ICourseStore
{
    // Constructors 
    public CourseStore(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }
}
