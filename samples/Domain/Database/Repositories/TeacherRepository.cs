using CoreSharp.EntityFramework.Repositories.Abstracts;
using Domain.Database.Models;
using Domain.Database.Repositories.Interfaces;
using System;

namespace Domain.Database.Repositories;

public class TeacherRepository : ExtendedRepositoryBase<Teacher, Guid>, ITeacherRepository
{
    // Constructors
    public TeacherRepository(AppDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }
}
