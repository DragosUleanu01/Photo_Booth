using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photo_Booth_Server_API.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeToImageFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ImageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ContentType",
                value: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ContentType",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ImageFiles");
        }
    }
}
