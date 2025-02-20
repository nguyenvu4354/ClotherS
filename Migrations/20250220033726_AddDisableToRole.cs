using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClotherS.Migrations
{
    /// <inheritdoc />
    public partial class AddDisableToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disable",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disable",
                table: "Roles");
        }
    }
}
