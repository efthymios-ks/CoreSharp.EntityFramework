using CoreSharp.EntityFramework.Stores.Abstracts;
using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Database.Stores;

public class TeacherStore(DbContext dbContext)
    : StoreBase<Teacher, Guid>(dbContext), ITeacherStore
{
}
