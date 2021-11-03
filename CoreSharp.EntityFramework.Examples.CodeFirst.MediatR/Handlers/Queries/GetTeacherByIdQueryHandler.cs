using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Queries
{
    public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, Teacher>
    {
        //Fields
        private readonly ITeacherRepository _teacherRepository;

        //Constructors
        public GetTeacherByIdQueryHandler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }

        //Methods
        public async Task<Teacher> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
            => await _teacherRepository.GetAsync(request.TeacherId, request.Navigation, cancellationToken);
    }
}
