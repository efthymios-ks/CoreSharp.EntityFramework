using Domain.Database.Models;
using MediatR.Queries.Abstract;
using System.Diagnostics.CodeAnalysis;

namespace MediatR.Queries;

[SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "<Pending>")]
public sealed record GetTeachersQuery : RepositoryNavigationBase<Teacher>, IRequest<IEnumerable<Teacher>>;
