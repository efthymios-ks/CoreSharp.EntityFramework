using CoreSharp.EntityFramework.Repositories.Abstracts;
using Domain.Database.Repositories;
using Domain.Database.Repositories.Interfaces;
using Domain.Database.UnitOfWorks.Interfaces;

namespace Domain.Database.UnitOfWorks;

public class AppUnitOfWork(AppDbContext schoolDbContext)
    : UnitOfWorkBase(schoolDbContext), IAppUnitOfWork
{
    // Fields 
    private ICourseRepository? _courses;
    private ITeacherRepository? _teachers;

    // Properties
    public ICourseRepository Courses
        => _courses ??= new CourseRepository(Context);
    public ITeacherRepository Teachers
        => _teachers ??= new TeacherRepository(Context);
}
