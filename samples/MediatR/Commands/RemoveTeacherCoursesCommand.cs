using Domain.Database.Models;

namespace MediatR.Commands;

public sealed record RemoveTeacherCoursesCommand(Guid TeacherId) : IRequest<Teacher>;
