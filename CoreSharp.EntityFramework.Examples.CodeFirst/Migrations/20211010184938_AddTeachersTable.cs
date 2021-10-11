using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Migrations
{
    public partial class AddTeachersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teacher_TeacherId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher");

            migrationBuilder.RenameTable(
                name: "Teacher",
                newName: "Teachers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teachers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TeacherTypes",
                columns: table => new
                {
                    Value = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherTypes", x => x.Value);
                });

            migrationBuilder.InsertData(
                table: "TeacherTypes",
                columns: new[] { "Value", "Name" },
                values: new object[,]
                {
                    { 0, "Elementary" },
                    { 1, "MiddleSchool" },
                    { 2, "HighSchool" },
                    { 3, "SpecialEducation" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherType",
                table: "Teachers",
                column: "TeacherType");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_TeacherTypes_TeacherType",
                table: "Teachers",
                column: "TeacherType",
                principalTable: "TeacherTypes",
                principalColumn: "Value",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_TeacherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_TeacherTypes_TeacherType",
                table: "Teachers");

            migrationBuilder.DropTable(
                name: "TeacherTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TeacherType",
                table: "Teachers");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "Teacher");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teacher_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
