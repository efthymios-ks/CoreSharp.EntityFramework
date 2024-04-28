# CoreSharp.EntityFramework 

[![Nuget](https://img.shields.io/nuget/v/CoreSharp.EntityFramework)](https://www.nuget.org/packages/CoreSharp.EntityFramework/)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=efthymios-ks_CoreSharp.EntityFramework&metric=coverage)](https://sonarcloud.io/summary/new_code?id=efthymios-ks_CoreSharp.EntityFramework)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=efthymios-ks_CoreSharp.EntityFramework&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=efthymios-ks_CoreSharp.EntityFramework)
![GitHub License](https://img.shields.io/github/license/efthymios-ks/CoreSharp.EntityFramework)

> A set of reusable and optimized code for EF Core.

## Features
- Implementations for `UnitOfWork` and `Repository` pattern.
- Implementations for `Store` pattern.
- Track and store DbContext changes.

## Installation
Install the package with [Nuget](https://www.nuget.org/packages/CoreSharp.EntityFramework/).  
```
dotnet add package CoreSharp.EntityFramework --version 7.5.0
```

## Terminology
- `Entity`: Represents a domain object or business object.
- `DbContext`: Manages database sessions and operations.
- `UnitOfWork` (UoW): Manages transactions and database connections (a wrapper around `DbContext`).
- `Repository`: Abstracts data access, typically **read-only**.
- `Store`: Manages data state, with **read** and **write** access.

## Documentation
### Important files
- Interfaces
    - [IRepository](/src/CoreSharp.EntityFramework/Repositories/Interfaces/IRepository%601.cs)
    - [IExtendedRepository](/src/CoreSharp.EntityFramework/Repositories/Interfaces/IExtendedRepository%601.cs)
    - [IUnitOfWork](/src/CoreSharp.EntityFramework/Repositories/Interfaces/IUnitOfWork.cs)
    - [IStore](/src/CoreSharp.EntityFramework/Stores/Interfaces/IStore%601.cs)
    - [IExtendedStore](/src/CoreSharp.EntityFramework/Stores/Interfaces/IExtendedStore%601.cs)
- Abstracts
    - [RepositoryBase](/src/CoreSharp.EntityFramework/Repositories/Abstracts/RepositoryBase%601.cs)
    - [ExtendedRepositoryBase](/src/CoreSharp.EntityFramework/Repositories/Abstracts/ExtendedRepositoryBase%601.cs)
    - [UnitOfWorkBase](/src/CoreSharp.EntityFramework/Repositories/Abstracts/UnitOfWorkBase.cs)
    - [StoreBase](/src/CoreSharp.EntityFramework/Stores/Abstracts/StoreBase%601.cs)
    - [ExtendedStoreBase](/src/CoreSharp.EntityFramework/Stores/Abstracts/ExtendedStoreBase%601.cs)
    - [AuditDbContextBase](src/CoreSharp.EntityFramework/DbContexts/Abstracts/AuditDbContextBase.cs)

### Query object
The [Query](/src/CoreSharp.EntityFramework/Delegates/Query%601.cs) object is just a convention for  
```
delegate IQueryable<TEntity> Query<TEntity>(IQueryable<TEntity> query);
```  
It is used optionally in `repository` and `store` overloads to adjust the `DbSet<TEntity>` before querying it.
```
var highSchoolTeacherIds = (await teacherRepository
    .GetAsync(query => query
    .Where(teacher => teacher.TeacherType == TeacherType.HighSchool) // Filter
    .Select(teacher => new Teacher // Project
    {
        Id = teacher.Id
    })
    .AsNoTracking())
    .Select(teacher => teacher.Id)
    .ToArray();
```

## Use cases
### Table of Contents
- [Repository pattern](#repository-pattern)
- [Store pattern](#store-pattern)

### Repository pattern
1. Define your entity.
```
using CoreSharp.EntityFramework.Entities.Abstracts;

public sealed class Teacher : EntityBase<Guid>
{
    // Properties
    public string Name { get; set; }
}
```
```
public sealed class SchoolDbContext : DbContext
{
    // Properties
    public DbSet<Teacher> Teachers { get; set; } 
}
```

2. Define your repository.
```
using CoreSharp.EntityFramework.Repositories.Interfaces;

public interface ITeacherRepository : IRepository<Teacher, Guid>
{
}
```
```
using CoreSharp.EntityFramework.Repositories.Abstracts;

public sealed class TeacherRepository : RepositoryBase<Teacher, Guid>, ITeacherRepository
{
    // Constructors
    public TeacherRepository(DbContext dbContext)
        : base(dbContext)
    {
    }
}
```

3. Define your UnitOfWork.
```
using CoreSharp.EntityFramework.Repositories.Interfaces;

public interface ISchoolUnitOfWork : IUnitOfWork
{
    // Properties
    ITeacherRepository Teachers { get; }
}
```
```
using CoreSharp.EntityFramework.Repositories.Abstracts;

public sealed class SchoolUnitOfWork : UnitOfWorkBase, ISchoolUnitOfWork
{
    // Fields
    private ITeacherRepository _teachers;

    // Constructors
    public AppUnitOfWork(SchoolDbContext schoolDbContext)
        : base(schoolDbContext)
    {
    }

    // Properties
    public ITeacherRepository Teachers
        => _teachers ??= new TeacherRepository(Context);
}
```

4. Register UnitOfWork.
```
public static IServiceProvider AddDatabase(this IServiceCollection serviceCollection)
{
    serviceCollection.AddScoped<ISchoolUnitOfWork, SchoolUnitOfWork>();

    return serviceCollection;
}
```

5. Inject and use UnitOfWork.
```
public sealed class SchoolManager
{
    private readonly ISchoolUnitOfWork _unitOfWork;

    public SchoolManager(ISchoolUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public Task<IEnumerable<Teacher>> GetTeachersAsync()
        => _unitOfWork.Teachers.GetAsync();

    public async Task AddTeachersAsync(Teacher teacherToAdd)
    {
        await _unitOfWork.Teachers.AddAsync(teacherToAdd);
        await _unitOfWork.CommitAsync();
    }
}
```

### Store pattern
1. Define your entity.
```
using CoreSharp.EntityFramework.Entities.Abstracts;

public sealed class Teacher : EntityBase<Guid>
{
    // Properties
    public string Name { get; set; }
}
```
```
public sealed class SchoolDbContext : DbContext
{
    // Properties
    public DbSet<Teacher> Teachers { get; set; } 
}
```

2. Define your store.
```
using CoreSharp.EntityFramework.Stores.Interfaces;

public interface ITeacherStore : IStore<Teacher, Guid>
{
}
```
```
using CoreSharp.EntityFramework.Stores.Abstracts;

public sealed class TeacherStore : StoreBase<Teacher, Guid>, ITeacherStore
{
    // Constructors
    public TeacherStore(DbContext dbContext)
        : base(dbContext)
    {
    }
}
```

3. Register store.
```
public static IServiceProvider AddDatabase(this IServiceCollection serviceCollection)
{
    serviceCollection.AddScoped<ISteacherStore, TeacherStore>();

    return serviceCollection;
}
```

4. Inject and use store.
```
public sealed class SchoolManager
{
    private readonly ITeacherStore _teacherStore;

    public SchoolManager(ITeacherStore teacherStore)
        => _teacherStore = teacherStore;

    public Task<IEnumerable<Teacher>> GetTeachersAsync()
        => _teacherStore.Teachers.GetAsync();

    public async Task AddTeachersAsync(Teacher teacherToAdd)
        => _teacherStore.Teachers.AddAsync(teacherToAdd); 
}
```
