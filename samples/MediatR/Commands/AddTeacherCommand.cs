using Domain.Database.Models;

namespace MediatR.Commands;

public sealed record AddTeacherCommand(Teacher Teacher) : IRequest<Teacher>;
