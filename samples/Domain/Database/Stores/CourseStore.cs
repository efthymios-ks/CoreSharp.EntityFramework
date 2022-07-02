using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using CoreSharp.EntityFramework.Samples.Domain.Database.Stores.Interfaces;
using CoreSharp.EntityFramework.Stores.Common;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Stores;

public class CourseStore : StoreBase<Course>, ICourseStore
{
    //Constructors 
    public CourseStore(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }
}
