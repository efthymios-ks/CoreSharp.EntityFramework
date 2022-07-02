using CoreSharp.EntityFramework.Repositories.Common;
using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using CoreSharp.EntityFramework.Samples.Domain.Database.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Repositories;

public class TeacherRepository : ExtendedRepositoryBase<Teacher>, ITeacherRepository
{
    //Constructors
    public TeacherRepository(AppDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }
}
