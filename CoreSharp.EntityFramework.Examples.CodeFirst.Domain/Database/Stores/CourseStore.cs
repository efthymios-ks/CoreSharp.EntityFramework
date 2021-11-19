using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores.Interfaces;
using CoreSharp.EntityFramework.Stores.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores
{
    public class CourseStore : StoreBase<Course>, ICourseStore
    {
        //Constructors 
        public CourseStore(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }
    }
}
