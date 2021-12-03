using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories
{
    public class CourseRepository : ExtendedRepositoryBase<Course>, ICourseRepository
    {
        //Constructors 
        public CourseRepository(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }
    }
}
