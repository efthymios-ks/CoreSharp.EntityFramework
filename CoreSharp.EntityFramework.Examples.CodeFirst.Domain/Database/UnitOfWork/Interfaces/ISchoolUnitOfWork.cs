using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces
{
    public interface ISchoolUnitOfWork : IUnitOfWork
    {
        //Properties
        public ICourseRepository Courses { get; }
        public ITeacherRepository Teachers { get; }
    }
}
