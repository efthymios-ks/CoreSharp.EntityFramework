using CoreSharp.EntityFramework.Repositories.Interfaces;
using Domain.Database.Repositories.Interfaces;

namespace Domain.Database.UnitOfWorks.Interfaces;

public interface IAppUnitOfWork : IUnitOfWork
{
    //Properties
    ICourseRepository Courses { get; }
    ITeacherRepository Teachers { get; }
}
