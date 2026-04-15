using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tickets");
        }
    }
}
