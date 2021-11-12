using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork
{
    public class SchoolUnitOfWork : UnitOfWorkBase, ISchoolUnitOfWork
    {
        //Fields 
        private readonly ICourseRepository _courses;
        private readonly ITeacherRepository _teachers;

        //Constructors
        public SchoolUnitOfWork(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }

        //Properties
        public ICourseRepository Courses => _courses ?? new CourseRepository(Context);
        public ITeacherRepository Teachers => _teachers ?? new TeacherRepository(Context);
    }
}
