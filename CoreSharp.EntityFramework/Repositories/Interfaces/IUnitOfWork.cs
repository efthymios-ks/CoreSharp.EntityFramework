using System;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Methods 
        Task CommitAsync();
        Task RollbackAsync();
    }
}
