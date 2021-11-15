using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork
{
    public class SchoolUnitOfWork : UnitOfWorkBase, ISchoolUnitOfWork
    {
        //Fields 
        private ICourseRepository _courses = null;
        private ITeacherRepository _teachers = null;

        //Constructors
        public SchoolUnitOfWork(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }

        //Properties
        public ICourseRepository Courses => _courses ??= new CourseRepository(Context as SchoolDbContext);
        public ITeacherRepository Teachers => _teachers ??= new TeacherRepository(Context as SchoolDbContext);
    }
}
