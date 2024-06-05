using Domain.Database.Models;
using MediatR.Queries.Abstract;

namespace MediatR.Queries;

public sealed record GetTeacherByIdQuery(Guid TeacherId) : RepositoryNavigationBase<Teacher>, IRequest<Teacher?>;
