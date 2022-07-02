using CoreSharp.EntityFramework.Repositories.Common;
using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using CoreSharp.EntityFramework.Samples.Domain.Database.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Repositories;

public class CourseRepository : ExtendedRepositoryBase<Course>, ICourseRepository
{
    //Constructors 
    public CourseRepository(AppDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }
}
