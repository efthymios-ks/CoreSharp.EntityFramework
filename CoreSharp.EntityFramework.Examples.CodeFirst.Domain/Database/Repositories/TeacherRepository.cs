using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories
{
    public class TeacherRepository : ExtendedRepositoryBase<Teacher>, ITeacherRepository
    {
        //Constructors
        public TeacherRepository(AppDbContext schoolDbContext)
            : base(schoolDbContext)
        {
        }
    }
}
