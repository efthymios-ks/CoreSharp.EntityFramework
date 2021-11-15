using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Stores.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores.Interfaces
{
    public interface ITeacherStore : IStore<Teacher>
    {
    }
}
