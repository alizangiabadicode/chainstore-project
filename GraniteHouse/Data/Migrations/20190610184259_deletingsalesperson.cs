using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChainStore.Data.Migrations
{
    public partial class deletingsalesperson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_SalesPersonId",
                table: "Appointments");
            

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SalesPersonId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SalesPersonId",
                table: "Appointments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesPersonId",
                table: "Appointments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SuccessfullAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppointmentId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessfullAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessfullAppointments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuccessfullAppointments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SalesPersonId",
                table: "Appointments",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessfullAppointments_AppointmentId",
                table: "SuccessfullAppointments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessfullAppointments_ProductId",
                table: "SuccessfullAppointments",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_SalesPersonId",
                table: "Appointments",
                column: "SalesPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
