using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplePubManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubdomainToOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subdomain",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subdomain",
                table: "Organizations");
        }
    }
}
