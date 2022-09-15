using CoreSharp.EntityFramework.Stores.Common;
using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;

namespace Domain.Database.Stores;

public class TeacherStore : StoreBase<Teacher>, ITeacherStore
{
    // Constructors
    public TeacherStore(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }
}
