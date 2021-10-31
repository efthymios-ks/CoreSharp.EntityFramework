using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Configurations;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Extensions;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database
{
    public class SchoolDbContext : DbContextBase
    {
        //Properties
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentAddress> StudentAddresses { get; set; }
        public DbSet<Course> Courses { get; set; }

        //Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .ConfigureSchoolDbContext(Configuration.ConnectionString)
                .EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            ConfigureEnums(modelBuilder);
            ConfigureModels(modelBuilder);
        }

        private static void ConfigureEnums(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.HasEnum<TeacherType>("TeacherTypes");
            modelBuilder.HasEnum<CourseField>("CourseFields");
        }

        private static void ConfigureModels(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new StudentAddressConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
        }
    }
}
