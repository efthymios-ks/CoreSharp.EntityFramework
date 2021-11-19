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
Inherit from `DbContextBase` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/DbContextBase.cs). 
``` 
using CoreSharp.EntityFramework.Models.Abstracts;
using Microsoft.EntityFrameworkCore; 

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database
{
    public class SchoolDbContext : DbContextBase
    {
        //Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            //Always call base method 
            base.OnModelCreating(modelBuilder);
            
            //Other configurations 
        }
    }
}
``` 

### 2. Create an `Entity`. 
Inherit from `EntityBase<TKey>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/EntityBase%601.cs). 
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
Inherit from `RepositoryBase<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Repositories/Abstracts/RepositoryBase%601.cs). 
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

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces
{
    public interface ISchoolUnitOfWork : IUnitOfWork
    {
        //Properties
        public ITeacherRepository Teachers { get; }
    }
}
```

### 6. Implement the `UnitOfWork`. 
Inherit from `UnitOfWorkBase` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Repositories/Abstracts/UnitOfWorkBase.cs). 
``` 
using CoreSharp.EntityFramework.Repositories.Abstracts;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork
{
    public class SchoolUnitOfWork : UnitOfWorkBase, ISchoolUnitOfWork
    {
        //Fields 
        private ITeacherRepository _teachers = null;

        //Constructors
        public SchoolUnitOfWork(SchoolDbContext schoolDbContext) : base(schoolDbContext)
        {
        }

        //Properties
        public ITeacherRepository Teachers => _teachers ??= new TeacherRepository(Context as SchoolDbContext);
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
        public static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            //1. Add DbContext 
            serviceCollection.AddDbContext<SchoolDbContext>();
            
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
    public class AddTeacherQueryHandler : IRequestHandler<AddTeacherCommand, Teacher>
    {
        //Fields
        private readonly ISchoolUnitOfWork _schoolUnitOfWork;

        //Constructors
        public AddTeacherQueryHandler(ISchoolUnitOfWork schoolUnitOfWork)
        {
            _schoolUnitOfWork = schoolUnitOfWork ?? throw new ArgumentNullException(nameof(schoolUnitOfWork));
        }

        //Methods
        public async Task<IEnumerable<Teacher>> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
        {
            var createdTeacher = await _schoolUnitOfWork.Teachers.AddAsync(request.Teacher, cancellationToken);
            await _schoolUnitOfWork.CommitAsync(cancellationToken);
            return createdTeacher;
        }
    }
}
```

## Steps for `Store` scenario 
### 1. Create database specific `DbContext`. 
Inherit from `DbContextBase` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/DbContextBase.cs). 
```
using CoreSharp.EntityFramework.Models.Abstracts;
using Microsoft.EntityFrameworkCore; 

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database
{
    public class SchoolDbContext : DbContextBase
    {
        //Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            //Always call base method 
            base.OnModelCreating(modelBuilder);
            
            //Other configurations... 
        }
    }
}
```

### 2. Create an `Entity`. 
Inherit from `EntityBase<TKey>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Models/Abstracts/EntityBase%601.cs). 
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
Inherit from `IStore<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Store/Interfaces/IStore%601.cs). 
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
Inherit from `StoreBase<TEntity>` [(link)](https://github.com/efthymios-ks/CoreSharp.EntityFramework/blob/master/CoreSharp.EntityFramework/Store/Abstracts/StoreBase%601.cs). 
```
using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Stores
{
    public class TeacherStore : StoreBase<Teacher>, ITeacherStore
    {
        //Constructors
        public TeacherStore(SchoolDbContext schoolDbContext) : base(schoolDbContext)
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
        public static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            //1. Add DbContext 
            serviceCollection.AddDbContext<SchoolDbContext>();
            
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
    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Teacher>
    {
        //Fields
        private readonly ITeacherStore _teacherStore;

        //Constructors
        public UpdateTeacherCommandHandler(ITeacherStore teacherStore)
        {
            _teacherStore = teacherStore ?? throw new ArgumentNullException(nameof(teacherStore));
        }

        //Methods
        public async Task<Teacher> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        { 
            var updatedTeacher = await _teacherStore.UpdateAsync(request.Teacher, cancellationToken);
            return updatedTeacher;
        }
    }
}
```
