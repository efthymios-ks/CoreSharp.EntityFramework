# CoreSharp.EntityFramework 

## Description 
`CoreSharp.EntityFramework` contains a set of interfaces, abstracts and model bases for the commonest Entity Framework use scenarios. 

## Features 
- Interfaces for `Entity`, `Repository`, `UnitOfWork` and `Store`. 
- Abstract base implementations for `Entity`, `Repository`, `UnitOfWork` and `Store`. 
- Various extensions for `enum`, conversions, `DbContext` etc. 

## Installation 
Install via [nuget](https://www.nuget.org/packages/CoreSharp.EntityFramework/).

## Steps for `UnitOfWork` scenario 
### 1. Create database specific `DbContext`. 
Implement with `DbContextBase` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/DbContextBase.cs). 
``` 
using CoreSharp.EntityFramework.Models.Abstracts;
using Microsoft.EntityFrameworkCore; 

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database
{
    public class AppDbContext : DbContextBase
    {
        //Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            //Always call base method first
            base.OnModelCreating(modelBuilder);
            
            //Other configurations 
        }
    }
}
``` 

### 2. Create an `Entity`. 
Implement with `EntityBase<TKey>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/EntityBase%601.cs). 
``` 
using CoreSharp.EntityFramework.Models.Abstracts; 
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models
{
    public class Teacher : EntityBase<Guid>
    {
    }
}
``` 

### 3. Create a `Repository` interface. 
Inherit from `IRepository<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Repositories/Interfaces/IRepository%601.cs). 
``` 
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
    }
}
``` 

### 4. Implement the `Repository`. 
Implement with `RepositoryBase<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Repositories/Abstracts/RepositoryBase%601.cs). 
``` 
using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories
{
    public class TeacherRepository : RepositoryBase<Teacher>, ITeacherRepository
    {
        //Constructors
        public TeacherRepository(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }        
        
        //Methods 
        //If needed, you may override base methods 
        public override Task RemoveAsync(Teacher entity, CancellationToken cancellationToken = default)
        {
            //Different approach 
        }
    }
}
```

### 5. Create a `UnitOfWork` interface. 
Inherit from `IUnitOfWork` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Repositories/Interfaces/IUnitOfWork.cs). 
``` 
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWorks.Interfaces
{
    public interface IAppUnitOfWork : IUnitOfWork
    {
        //Properties
        public ITeacherRepository Teachers { get; }
    }
}
```

### 6. Implement the `UnitOfWork`. 
Implement with `UnitOfWorkBase` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Repositories/Abstracts/UnitOfWorkBase.cs). 
``` 
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWorks
{
    public class AppUnitOfWork : UnitOfWorkBase, IAppUnitOfWork
    {
        //Fields 
        private ITeacherRepository _teachers = null;

        //Constructors
        public SchoolUnitOfWork(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }

        //Properties
        public ITeacherRepository Teachers 
            => _teachers ??= new TeacherRepository(Context as SchoolDbContext);
    }
}
```

### 7. Register everything. 
``` 
using CoreSharp.EntityFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    /// <summary>
    /// Pseudo-startup class.
    /// </summary>
    internal static class Startup
    {
        //Methods 
        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            //1. Add DbContext 
            serviceCollection.AddDbContext<AppDbContext>();
            
            //2. Add Repositories
            serviceCollection.AddRepositories(typeof(SchoolDbContext).Assembly);

            //3. Add UnitOfWork 
            serviceCollection.AddScoped<ISchoolUnitOfWork, SchoolUnitOfWork>();
            
            return serviceCollection.BuildServiceProvider();
        }
    }
}
```

### 8. Inject and use. 
``` 
namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Commands
{
    public class AddTeacherCommand : IRequest<Teacher>
    {
        //Constructors
        public AddTeacherCommand(Teacher teacher)
            => Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));

        //Properties
        public Teacher Teacher { get; }
    }

    public class AddTeacherCommandHandler : IRequestHandler<AddTeacherCommand, Teacher>
    {
        //Fields
        private readonly IAppUnitOfWork _appUnitOfWork;

        //Constructors
        public AddTeacherCommandHandler(IAppUnitOfWork appUnitOfWork)
            => _appUnitOfWork = appUnitOfWork;

        //Methods
        public async Task<Teacher> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
        {
            _ = request.Teacher ?? throw new NullReferenceException($"{nameof(request.Teacher)} cannot be null.");

            var createdTeacher = await _appUnitOfWork.Teachers.AddAsync(request.Teacher, cancellationToken);
            await _appUnitOfWork.CommitAsync(cancellationToken);
            return createdTeacher;
        }
    }
}
```

## Steps for `Store` scenario 
### 1. Create database specific `DbContext`. 
Implement with `DbContextBase` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/DbContextBase.cs). 
```
using CoreSharp.EntityFramework.Models.Abstracts;
using Microsoft.EntityFrameworkCore; 

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database
{
    public class AppDbContext : DbContextBase
    {
        //Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            //Always call base method first 
            base.OnModelCreating(modelBuilder);
            
            //Other configurations... 
        }
    }
}
```

### 2. Create an `Entity`. 
Implement with `EntityBase<TKey>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/EntityBase%601.cs). 
```
using CoreSharp.EntityFramework.Models.Abstracts; 
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models
{
    public class Teacher : EntityBase<Guid>
    {
    }
}
```

### 3. Create a `Store` interface. 
Inherit from `IStore<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Stores/Interfaces/IStore%601.cs). 
```
using CoreSharp.EntityFramework.Stores.Interfaces;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores.Interfaces
{
    public interface ITeacherStore : IStore<Teacher>
    {
    }
}
```

### 4. Implement the `Store`. 
Implement with `StoreBase<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Stores/Abstracts/StoreBase%601.cs). 
```
using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores
{
    public class TeacherStore : StoreBase<Teacher>, ITeacherStore
    {
        //Constructors
        public TeacherStore(AppDbContext appDbContext) 
            : base(appDbContext)
        {
        }        
        
        //Methods 
        //If needed, you may override base methods 
        public override Task RemoveAsync(Teacher entity, CancellationToken cancellationToken = default)
        {
            //Different approach 
        }
    }
}
```

### 5. Register everything. 
``` 
using CoreSharp.EntityFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    /// <summary>
    /// Pseudo-startup class.
    /// </summary>
    internal static class Startup
    {
        //Methods 
        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            //1. Add DbContext 
            serviceCollection.AddDbContext<AppDbContext>();
            
            //2. Add Stores
            serviceCollection.AddStores(typeof(SchoolDbContext).Assembly);

            return serviceCollection.BuildServiceProvider();
        }
    }
}
```

### 6. Inject and use. 
``` 
namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Commands
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
```
