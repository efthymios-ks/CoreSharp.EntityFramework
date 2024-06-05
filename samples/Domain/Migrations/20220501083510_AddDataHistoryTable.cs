using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreSharp.EntityFramework.Samples.Domain.Migrations;

public partial class AddDataHistoryTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "__EFDataHistory",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DateCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Keys = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PreviousState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NewState = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK___EFDataHistory", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "__EFDataHistory");
    }
}
