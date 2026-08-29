using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photo_Booth_Server_API.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptionMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nonce",
                table: "ImageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "ImageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "ImageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Nonce", "Salt", "Tag" },
                values: new object[] { "", "", "" });

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Nonce", "Salt", "Tag" },
                values: new object[] { "", "", "" });

            migrationBuilder.UpdateData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nonce", "Salt", "Tag" },
                values: new object[] { "", "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nonce",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "ImageFiles");
        }
    }
}
