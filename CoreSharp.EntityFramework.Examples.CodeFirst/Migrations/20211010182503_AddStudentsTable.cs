using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Migrations
{
    public partial class AddStudentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Student_StudentsId",
                table: "CourseStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAddresses_Student_StudentId",
                table: "StudentAddresses");

            migrationBuilder.DropIndex(
                name: "IX_StudentAddresses_StudentId",
                table: "StudentAddresses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Student_StudentAddressId",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StudentAddresses_StudentId",
                table: "StudentAddresses",
                column: "StudentId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Students_StudentAddressId",
                table: "Students",
                column: "StudentAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Students_StudentsId",
                table: "CourseStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAddresses_Students_StudentId",
                table: "StudentAddresses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StudentAddresses_StudentId",
                table: "StudentAddresses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Students_StudentAddressId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Student");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Student_StudentAddressId",
                table: "Student",
                column: "StudentAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddresses_StudentId",
                table: "StudentAddresses",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Student_StudentsId",
                table: "CourseStudent",
                column: "StudentsId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAddresses_Student_StudentId",
                table: "StudentAddresses",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentAddressId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
