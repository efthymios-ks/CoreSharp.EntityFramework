using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreSharp.EntityFramework.Samples.Domain.Migrations;

public partial class RenameStudentAddressColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Students_StudentAddresses_StudentAddressId",
            table: "Students");

        migrationBuilder.RenameColumn(
            name: "StudentAddressId",
            table: "Students",
            newName: "AddressId");

        migrationBuilder.RenameIndex(
            name: "IX_Students_StudentAddressId",
            table: "Students",
            newName: "IX_Students_AddressId");

        migrationBuilder.AddForeignKey(
            name: "FK_Students_StudentAddresses_AddressId",
            table: "Students",
            column: "AddressId",
            principalTable: "StudentAddresses",
            principalColumn: "StudentId",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Students_StudentAddresses_AddressId",
            table: "Students");

        migrationBuilder.RenameColumn(
            name: "AddressId",
            table: "Students",
            newName: "StudentAddressId");

        migrationBuilder.RenameIndex(
            name: "IX_Students_AddressId",
            table: "Students",
            newName: "IX_Students_StudentAddressId");

        migrationBuilder.AddForeignKey(
            name: "FK_Students_StudentAddresses_StudentAddressId",
            table: "Students",
            column: "StudentAddressId",
            principalTable: "StudentAddresses",
            principalColumn: "StudentId",
            onDelete: ReferentialAction.Restrict);
    }
}
