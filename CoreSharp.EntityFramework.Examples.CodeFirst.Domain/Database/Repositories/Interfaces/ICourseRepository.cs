using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces
{
    public interface ICourseRepository : IExtendedRepository<Course>
    {
    }
}
