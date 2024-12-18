using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMaster.Modules.Exercises.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OpenFormInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Exercises");

            migrationBuilder.CreateTable(
                name: "Essays",
                schema: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseHeaderInMotherLanguage = table.Column<bool>(type: "boolean", nullable: false),
                    MotherLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetLanguageLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TopicsOfSentences = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GrammarSection = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VerifiedByTeacher = table.Column<bool>(type: "boolean", nullable: false),
                    Exercise = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Essays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mails",
                schema: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseHeaderInMotherLanguage = table.Column<bool>(type: "boolean", nullable: false),
                    MotherLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetLanguageLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TopicsOfSentences = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GrammarSection = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VerifiedByTeacher = table.Column<bool>(type: "boolean", nullable: false),
                    Exercise = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SummariesOfText",
                schema: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseHeaderInMotherLanguage = table.Column<bool>(type: "boolean", nullable: false),
                    MotherLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetLanguageLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TopicsOfSentences = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GrammarSection = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VerifiedByTeacher = table.Column<bool>(type: "boolean", nullable: false),
                    Exercise = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummariesOfText", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Essays",
                schema: "Exercises");

            migrationBuilder.DropTable(
                name: "Mails",
                schema: "Exercises");

            migrationBuilder.DropTable(
                name: "SummariesOfText",
                schema: "Exercises");
        }
    }
}
