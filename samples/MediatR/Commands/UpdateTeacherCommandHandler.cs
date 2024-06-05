using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;

namespace MediatR.Commands;

public sealed class UpdateTeacherCommandHandler(ITeacherStore teacherStore) : IRequestHandler<UpdateTeacherCommand, Teacher>
{
    // Fields
    private readonly ITeacherStore _teacherStore = teacherStore;

    // Methods
    public async Task<Teacher> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        _ = request.Teacher ?? throw new InvalidOperationException($"{nameof(request.Teacher)} cannot be null.");

        return await _teacherStore.UpdateAsync(request.Teacher, cancellationToken);
    }
}
