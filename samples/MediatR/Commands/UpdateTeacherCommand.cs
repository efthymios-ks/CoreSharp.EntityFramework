using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using CoreSharp.EntityFramework.Samples.Domain.Database.Stores.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Samples.MediatR.Commands
{
    public class UpdateTeacherCommand : IRequest<Teacher>
    {
        //Constructors
        public UpdateTeacherCommand(Teacher teacher)
            => Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));

        //Properties
        public Teacher Teacher { get; }
    }

    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Teacher>
    {
        //Fields
        private readonly ITeacherStore _teacherStore;

        //Constructors
        public UpdateTeacherCommandHandler(ITeacherStore teacherStore)
            => _teacherStore = teacherStore;

        //Methods
        public async Task<Teacher> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            _ = request.Teacher ?? throw new NullReferenceException($"{nameof(request.Teacher)} cannot be null.");

            return await _teacherStore.UpdateAsync(request.Teacher, cancellationToken);
        }
    }
}
