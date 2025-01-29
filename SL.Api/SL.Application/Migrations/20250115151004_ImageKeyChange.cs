using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SL.Application.Migrations
{
    /// <inheritdoc />
    public partial class ImageKeyChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_OwnerWalletAddress",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "OwnerWalletAddress",
                table: "Images",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_OwnerWalletAddress",
                table: "Images",
                newName: "IX_Images_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_UserId",
                table: "Images",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_UserId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Images",
                newName: "OwnerWalletAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Images_UserId",
                table: "Images",
                newName: "IX_Images_OwnerWalletAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_OwnerWalletAddress",
                table: "Images",
                column: "OwnerWalletAddress",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
