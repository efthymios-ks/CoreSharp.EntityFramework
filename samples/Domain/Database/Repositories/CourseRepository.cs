using CoreSharp.EntityFramework.Repositories.Abstracts;
using Domain.Database.Models;
using Domain.Database.Repositories.Interfaces;
using System;

namespace Domain.Database.Repositories;

public class CourseRepository : ExtendedRepositoryBase<Course, Guid>, ICourseRepository
{
    // Constructors 
    public CourseRepository(AppDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }
}
