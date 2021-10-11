using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Database.Repositories
{
    internal class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        //Constructors
        public TeacherRepository(DbContext context) : base(context)
        {
        }
    }
}
