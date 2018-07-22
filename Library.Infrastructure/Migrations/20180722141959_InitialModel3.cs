using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.Infrastructure.Migrations
{
    public partial class InitialModel3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservedDate",
                table: "BookTracking",
                newName: "ReserveDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualReturnDate",
                table: "BookTracking",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualReturnDate",
                table: "BookTracking");

            migrationBuilder.RenameColumn(
                name: "ReserveDate",
                table: "BookTracking",
                newName: "ReservedDate");
        }
    }
}
