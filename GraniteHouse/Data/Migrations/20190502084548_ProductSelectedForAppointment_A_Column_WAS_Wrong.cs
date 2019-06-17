using Microsoft.EntityFrameworkCore.Migrations;

namespace ChainStore.Data.Migrations
{
    public partial class ProductSelectedForAppointment_A_Column_WAS_Wrong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsSelectedForAppointments_Products_AppointmentId",
                table: "ProductsSelectedForAppointments");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsSelectedForAppointments_ProductId",
                table: "ProductsSelectedForAppointments",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsSelectedForAppointments_Products_ProductId",
                table: "ProductsSelectedForAppointments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsSelectedForAppointments_Products_ProductId",
                table: "ProductsSelectedForAppointments");

            migrationBuilder.DropIndex(
                name: "IX_ProductsSelectedForAppointments_ProductId",
                table: "ProductsSelectedForAppointments");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsSelectedForAppointments_Products_AppointmentId",
                table: "ProductsSelectedForAppointments",
                column: "AppointmentId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
