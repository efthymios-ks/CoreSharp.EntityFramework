using CoreSharp.EntityFramework.Stores.Abstracts;
using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;

namespace Domain.Database.Stores;

public class CourseStore : StoreBase<Course>, ICourseStore
{
    // Constructors 
    public CourseStore(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }
}
