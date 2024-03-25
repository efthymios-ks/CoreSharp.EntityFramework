using CoreSharp.EntityFramework.Stores.Abstracts;
using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;
using System;

namespace Domain.Database.Stores;

public class TeacherStore : StoreBase<Teacher, Guid>, ITeacherStore
{
    // Constructors
    public TeacherStore(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }
}
