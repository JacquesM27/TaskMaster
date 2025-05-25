using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMaster.Modules.Accounts.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UserUniqueNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueNumber",
                schema: "Users",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueNumber",
                schema: "Users",
                table: "Users");
        }
    }
}
