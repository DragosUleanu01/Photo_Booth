using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Photo_Booth_Server_API.Migrations
{
    /// <inheritdoc />
    public partial class dataadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ImageFiles",
                columns: new[] { "Id", "FilePath", "Subject" },
                values: new object[,]
                {
                    { 1, "path/to/image1.jpg", "Sample Image 1" },
                    { 2, "path/to/image2.jpg", "Sample Image 2" },
                    { 3, "path/to/image3.jpg", "Sample Image 3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ImageFiles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
