using CoreSharp.EntityFramework.Repositories.Abstracts;
using Domain.Database.Models;
using Domain.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Database.Repositories;

public class TeacherRepository(DbContext dbContext)
    : ExtendedRepositoryBase<Teacher, Guid>(dbContext), ITeacherRepository
{
}
