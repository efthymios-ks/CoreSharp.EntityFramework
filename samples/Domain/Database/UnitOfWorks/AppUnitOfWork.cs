using CoreSharp.EntityFramework.Repositories.Common;
using CoreSharp.EntityFramework.Samples.Domain.Database.Repositories;
using CoreSharp.EntityFramework.Samples.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Samples.Domain.Database.UnitOfWorks.Interfaces;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.UnitOfWorks
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
