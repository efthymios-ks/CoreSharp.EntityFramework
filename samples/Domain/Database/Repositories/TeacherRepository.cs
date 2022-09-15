using CoreSharp.EntityFramework.Repositories.Common;
using Domain.Database.Models;
using Domain.Database.Repositories.Interfaces;

namespace Domain.Database.Repositories;

public class TeacherRepository : ExtendedRepositoryBase<Teacher>, ITeacherRepository
{
    // Constructors
    public TeacherRepository(AppDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }
}
