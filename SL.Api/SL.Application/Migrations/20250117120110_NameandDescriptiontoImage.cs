using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SL.Application.Migrations
{
    /// <inheritdoc />
    public partial class NameandDescriptiontoImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Images");
        }
    }
}
