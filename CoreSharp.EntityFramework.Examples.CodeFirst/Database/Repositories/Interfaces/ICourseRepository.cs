using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Database.Repositories.Interfaces
{
    internal interface ICourseRepository : IRepository<Course>
    {
    }
}
