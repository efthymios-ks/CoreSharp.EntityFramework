using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores.Interfaces;
using CoreSharp.EntityFramework.Stores.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores
{
    public class TeacherStore : StoreBase<Teacher>, ITeacherStore
    {
        //Constructors
        public TeacherStore(AppDbContext schoolDbContext)
            : base(schoolDbContext)
        {
        }
    }
}
