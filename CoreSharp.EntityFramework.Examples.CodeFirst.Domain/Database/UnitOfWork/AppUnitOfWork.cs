using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork
{
    public class AppUnitOfWork : UnitOfWorkBase, IAppUnitOfWork
    {
        //Fields 
        private ICourseRepository _courses;
        private ITeacherRepository _teachers;

        //Constructors
        public AppUnitOfWork(AppDbContext schoolDbContext)
            : base(schoolDbContext)
        {
        }

        //Properties
        public ICourseRepository Courses
            => _courses ??= new CourseRepository(Context as AppDbContext);
        public ITeacherRepository Teachers
            => _teachers ??= new TeacherRepository(Context as AppDbContext);
    }
}
