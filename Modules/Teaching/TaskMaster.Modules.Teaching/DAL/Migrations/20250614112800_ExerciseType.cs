using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMaster.Modules.Teaching.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ExerciseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExerciseType",
                schema: "Teaching",
                table: "AssignmentExercises",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseType",
                schema: "Teaching",
                table: "AssignmentExercises");
        }
    }
}
