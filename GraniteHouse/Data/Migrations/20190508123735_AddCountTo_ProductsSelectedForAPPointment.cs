using Microsoft.EntityFrameworkCore.Migrations;

namespace ChainStore.Data.Migrations
{
    public partial class AddCountTo_ProductsSelectedForAPPointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "ProductsSelectedForAppointments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "ProductsSelectedForAppointments");
        }
    }
}
