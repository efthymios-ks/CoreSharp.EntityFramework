using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Commands
{
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
