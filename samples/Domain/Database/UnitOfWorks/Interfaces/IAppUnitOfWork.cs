using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Samples.Domain.Database.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.UnitOfWorks.Interfaces;

public interface IAppUnitOfWork : IUnitOfWork
{
    //Properties
    ICourseRepository Courses { get; }
    ITeacherRepository Teachers { get; }
}
