using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMaster.Modules.Teaching.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TeachingUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                schema: "Teaching",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "Teaching",
                table: "Assignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                schema: "Teaching",
                table: "ClassAssignments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "Teaching",
                table: "ClassAssignments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                schema: "Teaching",
                table: "ClassAssignments");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "Teaching",
                table: "ClassAssignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                schema: "Teaching",
                table: "Assignments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "Teaching",
                table: "Assignments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
