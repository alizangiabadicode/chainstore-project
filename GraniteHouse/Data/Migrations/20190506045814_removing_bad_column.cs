using Microsoft.EntityFrameworkCore.Migrations;

namespace ChainStore.Data.Migrations
{
    public partial class removing_bad_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShadeColor",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShadeColor",
                table: "Products",
                nullable: true);
        }
    }
}
