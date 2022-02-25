using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWorks.Interfaces
{
    public interface IAppUnitOfWork : IUnitOfWork
    {
        //Properties
        ICourseRepository Courses { get; }
        ITeacherRepository Teachers { get; }
    }
}
