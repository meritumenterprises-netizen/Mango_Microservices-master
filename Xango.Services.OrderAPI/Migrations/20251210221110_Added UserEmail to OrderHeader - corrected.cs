using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xango.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserEmailtoOrderHeadercorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "OrderHeaders");
        }
    }
}
