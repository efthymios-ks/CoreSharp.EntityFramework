using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
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
        private readonly ISchoolUnitOfWork _schoolUnitOfWork;

        //Constructors
        public GetTeachersQueryHandler(ISchoolUnitOfWork schoolUnitOfWork)
        {
            _schoolUnitOfWork = schoolUnitOfWork ?? throw new ArgumentNullException(nameof(schoolUnitOfWork));
        }

        //Methods
        public async Task<IEnumerable<Teacher>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
            => await _schoolUnitOfWork.Teachers.GetAsync(navigation: request.Navigation, cancellationToken: cancellationToken);
    }
}
