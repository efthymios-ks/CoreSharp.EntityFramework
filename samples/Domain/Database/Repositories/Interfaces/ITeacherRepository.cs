using CoreSharp.EntityFramework.Repositories.Interfaces;
using Domain.Database.Models;
using System;

namespace Domain.Database.Repositories.Interfaces;

public interface ITeacherRepository : IExtendedRepository<Teacher, Guid>
{
}
