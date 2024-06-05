using CoreSharp.EntityFramework.Repositories.Interfaces;
using Domain.Database.Models;

namespace Domain.Database.Repositories.Interfaces;

public interface ICourseRepository : IExtendedRepository<Course, Guid>
{
}
