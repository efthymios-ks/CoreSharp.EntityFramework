﻿// <auto-generated />
using Domain.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CoreSharp.EntityFramework.Samples.Domain.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211011135932_AddFieldsToCourse")]
    partial class AddFieldsToCourse
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModifiedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fields")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("TeacherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TeacherId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModifiedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .IsUnique();

                    b.ToTable("Students");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.StudentAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModifiedUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("PostCode")
                        .HasColumnType("int");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("StudentAddresses");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Teacher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModifiedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TeacherType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TeacherType");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Models.Concrete.EnumShadowEntity<CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.CourseField>", b =>
                {
                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Value");

                    b.ToTable("CourseFields");

                    b.HasData(
                        new
                        {
                            Value = 0,
                            Name = "ChemicalEngineering"
                        },
                        new
                        {
                            Value = 1,
                            Name = "CivilEngineering"
                        },
                        new
                        {
                            Value = 2,
                            Name = "ComputerEngineering"
                        },
                        new
                        {
                            Value = 3,
                            Name = "ElectricalEngineering"
                        },
                        new
                        {
                            Value = 4,
                            Name = "ElectronicEngineering"
                        });
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Models.Concrete.EnumShadowEntity<CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.TeacherType>", b =>
                {
                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Value");

                    b.ToTable("TeacherTypes");

                    b.HasData(
                        new
                        {
                            Value = 0,
                            Name = "Elementary"
                        },
                        new
                        {
                            Value = 1,
                            Name = "MiddleSchool"
                        },
                        new
                        {
                            Value = 2,
                            Name = "HighSchool"
                        },
                        new
                        {
                            Value = 3,
                            Name = "SpecialEducation"
                        });
                });

            modelBuilder.Entity("CourseStudent", b =>
                {
                    b.Property<Guid>("CoursesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CoursesId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("StudentsCourses");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Course", b =>
                {
                    b.HasOne("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Teacher", "Teacher")
                        .WithMany("Courses")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Student", b =>
                {
                    b.HasOne("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.StudentAddress", "Address")
                        .WithOne("Student")
                        .HasForeignKey("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Student", "AddressId")
                        .HasPrincipalKey("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.StudentAddress", "StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Teacher", b =>
                {
                    b.HasOne("CoreSharp.EntityFramework.Models.Concrete.EnumShadowEntity<CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.TeacherType>", null)
                        .WithMany()
                        .HasForeignKey("TeacherType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CourseStudent", b =>
                {
                    b.HasOne("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Course", null)
                        .WithMany()
                        .HasForeignKey("CoursesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.StudentAddress", b =>
                {
                    b.Navigation("Student");
                });

            modelBuilder.Entity("CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models.Teacher", b =>
                {
                    b.Navigation("Courses");
                });
#pragma warning restore 612, 618
        }
    }
}
