using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores.Interfaces;
using CoreSharp.EntityFramework.Stores.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores
{
    internal class TeacherStore : StoreBase<Teacher>, ITeacherStore
    {
        //Constructors
        public TeacherStore(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }
    }
}
