using CoreSharp.EntityFramework.Stores.Interfaces;
using Domain.Database.Models;

namespace Domain.Database.Stores.Interfaces;

public interface ICourseStore : IStore<Course, Guid>
{
}
