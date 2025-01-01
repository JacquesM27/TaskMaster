using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMaster.Modules.Teaching.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TeachingInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Teaching");

            migrationBuilder.CreateTable(
                name: "Assignments",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EssayAnswers",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WrittenEssay = table.Column<string>(type: "text", nullable: false),
                    AssignmentExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EssayAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailAnswers",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WrittenMail = table.Column<string>(type: "text", nullable: false),
                    AssignmentExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SummaryOfTextAnswers",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WrittenSummary = table.Column<string>(type: "text", nullable: false),
                    AssignmentExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryOfTextAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentExercises",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentExercises_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "Teaching",
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EssayAnswerMistakes",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EssayAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsAnswer = table.Column<string>(type: "text", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EssayAnswerMistakes", x => new { x.EssayAnswerId, x.Id });
                    table.ForeignKey(
                        name: "FK_EssayAnswerMistakes_EssayAnswers_EssayAnswerId",
                        column: x => x.EssayAnswerId,
                        principalSchema: "Teaching",
                        principalTable: "EssayAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailAnswerMistakes",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsAnswer = table.Column<string>(type: "text", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAnswerMistakes", x => new { x.MailAnswerId, x.Id });
                    table.ForeignKey(
                        name: "FK_MailAnswerMistakes_MailAnswers_MailAnswerId",
                        column: x => x.MailAnswerId,
                        principalSchema: "Teaching",
                        principalTable: "MailAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolAdmins",
                schema: "Teaching",
                columns: table => new
                {
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolAdmins", x => new { x.SchoolId, x.AdminId });
                    table.ForeignKey(
                        name: "FK_SchoolAdmins_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalSchema: "Teaching",
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SchoolTeachers",
                schema: "Teaching",
                columns: table => new
                {
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTeachers", x => new { x.SchoolId, x.TeacherId });
                    table.ForeignKey(
                        name: "FK_SchoolTeachers_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalSchema: "Teaching",
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeachingClasses",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MainTeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubTeachersIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    StudentsIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeachingClasses_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalSchema: "Teaching",
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeachingClasses_Schools_SchoolId1",
                        column: x => x.SchoolId1,
                        principalSchema: "Teaching",
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SummaryOfTextAnswerMistakes",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryOfTextAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsAnswer = table.Column<string>(type: "text", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryOfTextAnswerMistakes", x => new { x.SummaryOfTextAnswerId, x.Id });
                    table.ForeignKey(
                        name: "FK_SummaryOfTextAnswerMistakes_SummaryOfTextAnswers_SummaryOfT~",
                        column: x => x.SummaryOfTextAnswerId,
                        principalSchema: "Teaching",
                        principalTable: "SummaryOfTextAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassAssignments",
                schema: "Teaching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeachingClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassAssignments_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "Teaching",
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassAssignments_TeachingClasses_TeachingClassId",
                        column: x => x.TeachingClassId,
                        principalSchema: "Teaching",
                        principalTable: "TeachingClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentExercises_AssignmentId_ExerciseId",
                schema: "Teaching",
                table: "AssignmentExercises",
                columns: new[] { "AssignmentId", "ExerciseId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssignments_AssignmentId",
                schema: "Teaching",
                table: "ClassAssignments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssignments_TeachingClassId",
                schema: "Teaching",
                table: "ClassAssignments",
                column: "TeachingClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingClasses_SchoolId",
                schema: "Teaching",
                table: "TeachingClasses",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingClasses_SchoolId1",
                schema: "Teaching",
                table: "TeachingClasses",
                column: "SchoolId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentExercises",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "ClassAssignments",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "EssayAnswerMistakes",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "MailAnswerMistakes",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "SchoolAdmins",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "SchoolTeachers",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "SummaryOfTextAnswerMistakes",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "Assignments",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "TeachingClasses",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "EssayAnswers",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "MailAnswers",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "SummaryOfTextAnswers",
                schema: "Teaching");

            migrationBuilder.DropTable(
                name: "Schools",
                schema: "Teaching");
        }
    }
}
