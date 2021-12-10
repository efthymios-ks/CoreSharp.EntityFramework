using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries.Abstract;
using MediatR;
using System;

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
}
