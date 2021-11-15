using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories
{
    internal class TeacherRepository : RepositoryBase<Teacher>, ITeacherRepository
    {
        //Constructors
        public TeacherRepository(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }
    }
}
