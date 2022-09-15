using CoreSharp.EntityFramework.Repositories.Common;
using Domain.Database.Models;
using Domain.Database.Repositories.Interfaces;

namespace Domain.Database.Repositories;

public class CourseRepository : ExtendedRepositoryBase<Course>, ICourseRepository
{
    // Constructors 
    public CourseRepository(AppDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }
}
