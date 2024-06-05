using Domain.Database.Models;

namespace MediatR.Commands;

public sealed record UpdateTeacherCommand(Teacher Teacher) : IRequest<Teacher>;
