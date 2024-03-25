using CoreSharp.EntityFramework.Stores.Interfaces;
using Domain.Database.Models;
using System;

namespace Domain.Database.Stores.Interfaces;

public interface ITeacherStore : IStore<Teacher, Guid>
{
}
