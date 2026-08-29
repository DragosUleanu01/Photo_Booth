using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photo_Booth_Server_API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ImageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ImageFiles");
        }
    }
}
