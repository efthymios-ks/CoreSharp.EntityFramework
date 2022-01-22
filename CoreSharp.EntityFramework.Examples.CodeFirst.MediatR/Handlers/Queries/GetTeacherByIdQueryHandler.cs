using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Queries
{
    public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, Teacher>
    {
        //Fields
        private readonly IAppUnitOfWork _schoolUnitOfWork;

        //Constructors
        public GetTeacherByIdQueryHandler(IAppUnitOfWork schoolUnitOfWork)
            => _schoolUnitOfWork = schoolUnitOfWork;

        //Methods
        public async Task<Teacher> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
            => await _schoolUnitOfWork.Teachers.GetAsync(request.TeacherId, request.Navigation, cancellationToken);
    }
}
