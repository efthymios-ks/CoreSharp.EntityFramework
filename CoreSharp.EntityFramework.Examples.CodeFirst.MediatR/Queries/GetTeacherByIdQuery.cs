using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries.Abstract;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries
{
    public class GetTeacherByIdQuery : RepositoryNavigationBase<Teacher>, IRequest<Teacher>
    {
        //Constructors
        public GetTeacherByIdQuery(Guid teacherId)
            => TeacherId = teacherId;

        //Properties
        public Guid TeacherId { get; }
    }

    public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, Teacher>
    {
        //Fields
        private readonly IAppUnitOfWork _appUnitOfWork;

        //Constructors
        public GetTeacherByIdQueryHandler(IAppUnitOfWork appUnitOfWork)
            => _appUnitOfWork = appUnitOfWork;

        //Methods
        public async Task<Teacher> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
            => await _appUnitOfWork.Teachers.GetAsync(request.TeacherId, request.Navigation, cancellationToken);
    }
}
