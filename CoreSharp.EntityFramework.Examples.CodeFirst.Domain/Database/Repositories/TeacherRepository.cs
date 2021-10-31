using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories
{
    internal class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        //Constructors
        public TeacherRepository(DbContext context) : base(context)
        {
        }
    }
}
