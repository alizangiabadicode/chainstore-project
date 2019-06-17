using Microsoft.EntityFrameworkCore.Migrations;

namespace ChainStore.Data.Migrations
{
    public partial class Add_SpecialTags_Foreingkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialTagId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecialTagId",
                table: "Products",
                column: "SpecialTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagId",
                table: "Products",
                column: "SpecialTagId",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecialTagId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecialTagId",
                table: "Products");
        }
    }
}
