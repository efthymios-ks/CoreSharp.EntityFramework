using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Queries
{
    public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, IEnumerable<Teacher>>
    {
        //Fields
        private readonly ITeacherRepository _teacherRepository;

        //Constructors
        public GetTeachersQueryHandler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }

        //Methods
        public async Task<IEnumerable<Teacher>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
            => await _teacherRepository.GetAsync(navigation: request.Navigation);
    }
}
