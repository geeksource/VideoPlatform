using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionService.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceIdSubsciption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "UserSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "UserSubscriptions");
        }
    }
}
