using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SL.Application.Migrations
{
    /// <inheritdoc />
    public partial class NftandImagemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nfts_Users_OwnerWalletAddress",
                table: "Nfts");

            migrationBuilder.RenameColumn(
                name: "OwnerWalletAddress",
                table: "Nfts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Nfts_OwnerWalletAddress",
                table: "Nfts",
                newName: "IX_Nfts_UserId");

            migrationBuilder.AddColumn<string>(
                name: "BlockchainTransactionHash",
                table: "Nfts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsTransferred",
                table: "Nfts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MintedAt",
                table: "Nfts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nfts_Users_UserId",
                table: "Nfts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nfts_Users_UserId",
                table: "Nfts");

            migrationBuilder.DropColumn(
                name: "BlockchainTransactionHash",
                table: "Nfts");

            migrationBuilder.DropColumn(
                name: "IsTransferred",
                table: "Nfts");

            migrationBuilder.DropColumn(
                name: "MintedAt",
                table: "Nfts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Nfts",
                newName: "OwnerWalletAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Nfts_UserId",
                table: "Nfts",
                newName: "IX_Nfts_OwnerWalletAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_Nfts_Users_OwnerWalletAddress",
                table: "Nfts",
                column: "OwnerWalletAddress",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
