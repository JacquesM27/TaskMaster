using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMaster.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CommunicationInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "infrastructure");

            migrationBuilder.CreateTable(
                name: "DomainEvents",
                schema: "infrastructure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEvents",
                schema: "infrastructure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "infrastructure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventData = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_AggregateId",
                schema: "infrastructure",
                table: "DomainEvents",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_AggregateId_Version",
                schema: "infrastructure",
                table: "DomainEvents",
                columns: new[] { "AggregateId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_CreatedAt",
                schema: "infrastructure",
                table: "DomainEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_EventType",
                schema: "infrastructure",
                table: "DomainEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEvents_CreatedAt",
                schema: "infrastructure",
                table: "IntegrationEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEvents_EventType",
                schema: "infrastructure",
                table: "IntegrationEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEvents_EventType_CreatedAt",
                schema: "infrastructure",
                table: "IntegrationEvents",
                columns: new[] { "EventType", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedAt",
                schema: "infrastructure",
                table: "OutboxMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EventType",
                schema: "infrastructure",
                table: "OutboxMessages",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAt",
                schema: "infrastructure",
                table: "OutboxMessages",
                column: "ProcessedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents",
                schema: "infrastructure");

            migrationBuilder.DropTable(
                name: "IntegrationEvents",
                schema: "infrastructure");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "infrastructure");
        }
    }
}
