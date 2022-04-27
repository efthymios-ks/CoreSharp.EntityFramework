using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreSharp.EntityFramework.Samples.Domain.Migrations
{
    public partial class AddCoursesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Teacher_TeacherId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Course_CoursesId",
                table: "CourseStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Students_StudentsId",
                table: "CourseStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAddresses_Students_StudentId",
                table: "StudentAddresses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Students_StudentAddressId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStudent",
                table: "CourseStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Course",
                table: "Course");

            migrationBuilder.RenameTable(
                name: "CourseStudent",
                newName: "StudentsCourses");

            migrationBuilder.RenameTable(
                name: "Course",
                newName: "Courses");

            migrationBuilder.RenameIndex(
                name: "IX_CourseStudent_StudentsId",
                table: "StudentsCourses",
                newName: "IX_StudentsCourses_StudentsId");

            migrationBuilder.RenameIndex(
                name: "IX_Course_TeacherId",
                table: "Courses",
                newName: "IX_Courses_TeacherId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsCourses",
                table: "StudentsCourses",
                columns: new[] { "CoursesId", "StudentsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentAddressId",
                table: "Students",
                column: "StudentAddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teacher_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAddresses_StudentAddressId",
                table: "Students",
                column: "StudentAddressId",
                principalTable: "StudentAddresses",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsCourses_Courses_CoursesId",
                table: "StudentsCourses",
                column: "CoursesId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsCourses_Students_StudentsId",
                table: "StudentsCourses",
                column: "StudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teacher_TeacherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAddresses_StudentAddressId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsCourses_Courses_CoursesId",
                table: "StudentsCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsCourses_Students_StudentsId",
                table: "StudentsCourses");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentAddressId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsCourses",
                table: "StudentsCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "StudentsCourses",
                newName: "CourseStudent");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "Course");

            migrationBuilder.RenameIndex(
                name: "IX_StudentsCourses_StudentsId",
                table: "CourseStudent",
                newName: "IX_CourseStudent_StudentsId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_TeacherId",
                table: "Course",
                newName: "IX_Course_TeacherId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Students_StudentAddressId",
                table: "Students",
                column: "StudentAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStudent",
                table: "CourseStudent",
                columns: new[] { "CoursesId", "StudentsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Course",
                table: "Course",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Teacher_TeacherId",
                table: "Course",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Course_CoursesId",
                table: "CourseStudent",
                column: "CoursesId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Students_StudentsId",
                table: "CourseStudent",
                column: "StudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAddresses_Students_StudentId",
                table: "StudentAddresses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentAddressId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
