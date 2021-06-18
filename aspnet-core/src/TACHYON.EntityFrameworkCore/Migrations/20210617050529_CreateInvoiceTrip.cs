using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreateInvoiceTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsAccountReceivable",
                table: "Invoices");

            migrationBuilder.AddColumn<byte>(
                name: "AccountType",
                table: "Invoices",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoiceTrips",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(nullable: false),
                    TripId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceTrips_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceTrips_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTrips_InvoiceId",
                table: "InvoiceTrips",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTrips_TripId",
                table: "InvoiceTrips",
                column: "TripId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceTrips");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccountReceivable",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InvoiceShippingRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    RequestId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceShippingRequests_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceShippingRequests_ShippingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceShippingRequests_InvoiceId",
                table: "InvoiceShippingRequests",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceShippingRequests_RequestId",
                table: "InvoiceShippingRequests",
                column: "RequestId");
        }
    }
}
