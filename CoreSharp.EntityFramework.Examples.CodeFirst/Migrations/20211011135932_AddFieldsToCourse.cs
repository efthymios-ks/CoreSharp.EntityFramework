using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Migrations
{
    public partial class AddFieldsToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fields",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseFields",
                columns: table => new
                {
                    Value = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseFields", x => x.Value);
                });

            migrationBuilder.InsertData(
                table: "CourseFields",
                columns: new[] { "Value", "Name" },
                values: new object[,]
                {
                    { 0, "ChemicalEngineering" },
                    { 1, "CivilEngineering" },
                    { 2, "ComputerEngineering" },
                    { 3, "ElectricalEngineering" },
                    { 4, "ElectronicEngineering" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseFields");

            migrationBuilder.DropColumn(
                name: "Fields",
                table: "Courses");
        }
    }
}
