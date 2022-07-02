using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using CoreSharp.EntityFramework.Samples.Domain.Database.Stores.Interfaces;
using CoreSharp.EntityFramework.Stores.Common;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Stores
{
    public class TeacherStore : StoreBase<Teacher>, ITeacherStore
    {
        //Constructors
        public TeacherStore(AppDbContext appDbContext)
            : base(appDbContext)
        {
        }
    }
}
