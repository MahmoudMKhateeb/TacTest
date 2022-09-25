using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Broker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypes_DocumentsEntities_DocumentsEntityId",
                table: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "DocumentsEntities");

            //migrationBuilder.DropIndex(
            //    name: "IX_DocumentTypes_DocumentsEntityId",
            //    table: "DocumentTypes");

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ActorInvoiceId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperActorId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "PriceOffers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmitInvoiceNumber",
                table: "InvoiceNotes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperActorId",
                table: "Facilities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Flag",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActorId",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActorCarrierPrice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ShippingRequestTripId = table.Column<int>(nullable: true),
                    ShippingRequestTripVasId = table.Column<long>(nullable: true),
                    IsActorCarrierHaveInvoice = table.Column<bool>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: true),
                    VatAmount = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorCarrierPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorCarrierPrice_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorCarrierPrice_ShippingRequestTripVases_ShippingRequestTripVasId",
                        column: x => x.ShippingRequestTripVasId,
                        principalTable: "ShippingRequestTripVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CompanyName = table.Column<string>(maxLength: 255, nullable: false),
                    ActorType = table.Column<byte>(nullable: false),
                    MoiNumber = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Logo = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    OrganizationUnitId = table.Column<long>(nullable: false),
                    InvoiceDueDays = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActorShipperPrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ShippingRequestTripId = table.Column<int>(nullable: true),
                    ShippingRequestTripVasId = table.Column<long>(nullable: true),
                    IsActorShipperHaveInvoice = table.Column<bool>(nullable: false),
                    TotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    SubTotalAmountWithCommission = table.Column<decimal>(nullable: true),
                    VatAmountWithCommission = table.Column<decimal>(nullable: true),
                    TaxVat = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorShipperPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                        column: x => x.ShippingRequestTripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorShipperPrices_ShippingRequestTripVases_ShippingRequestTripVasId",
                        column: x => x.ShippingRequestTripVasId,
                        principalTable: "ShippingRequestTripVases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActorInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    InvoiceNumber = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    ShipperActorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorInvoices_Actors_ShipperActorId",
                        column: x => x.ShipperActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActorInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_CarrierActorId",
                table: "Trucks",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorInvoiceId",
                table: "ShippingRequestTrips",
                column: "ActorInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_CarrierActorId",
                table: "ShippingRequests",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShipperActorId",
                table: "ShippingRequests",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOffers_CarrierActorId",
                table: "PriceOffers",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ShipperActorId",
                table: "Facilities",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_ActorId",
                table: "DocumentFiles",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CarrierActorId",
                table: "AbpUsers",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrice_ShippingRequestTripId",
                table: "ActorCarrierPrice",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrice_ShippingRequestTripVasId",
                table: "ActorCarrierPrice",
                column: "ShippingRequestTripVasId",
                unique: true,
                filter: "[ShippingRequestTripVasId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorInvoices_ShipperActorId",
                table: "ActorInvoices",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorInvoices_TenantId",
                table: "ActorInvoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_TenantId",
                table: "Actors",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripVasId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripVasId",
                unique: true,
                filter: "[ShippingRequestTripVasId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Actors_CarrierActorId",
                table: "AbpUsers",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_Actors_ActorId",
                table: "DocumentFiles",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Actors_ShipperActorId",
                table: "Facilities",
                column: "ShipperActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Facilities_AbpTenants_TenantId",
            //    table: "Facilities",
            //    column: "TenantId",
            //    principalTable: "AbpTenants",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceOffers_Actors_CarrierActorId",
                table: "PriceOffers",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receivers_AbpTenants_TenantId",
                table: "Receivers",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Actors_CarrierActorId",
                table: "ShippingRequests",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Actors_ShipperActorId",
                table: "ShippingRequests",
                column: "ShipperActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ActorInvoices_ActorInvoiceId",
                table: "ShippingRequestTrips",
                column: "ActorInvoiceId",
                principalTable: "ActorInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Actors_CarrierActorId",
                table: "Trucks",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Actors_CarrierActorId",
                table: "AbpUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_Actors_ActorId",
                table: "DocumentFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Actors_ShipperActorId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_AbpTenants_TenantId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceOffers_Actors_CarrierActorId",
                table: "PriceOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivers_AbpTenants_TenantId",
                table: "Receivers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Actors_CarrierActorId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Actors_ShipperActorId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorInvoices_ActorInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Actors_CarrierActorId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "ActorCarrierPrice");

            migrationBuilder.DropTable(
                name: "ActorInvoices");

            migrationBuilder.DropTable(
                name: "ActorShipperPrices");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_CarrierActorId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_CarrierActorId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShipperActorId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_PriceOffers_CarrierActorId",
                table: "PriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_ShipperActorId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_ActorId",
                table: "DocumentFiles");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_CarrierActorId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "ActorInvoiceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShipperActorId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "SubmitInvoiceNumber",
                table: "InvoiceNotes");

            migrationBuilder.DropColumn(
                name: "ShipperActorId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "AbpUsers");

            migrationBuilder.CreateTable(
                name: "DocumentsEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_DocumentsEntityId",
                table: "DocumentTypes",
                column: "DocumentsEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypes_DocumentsEntities_DocumentsEntityId",
                table: "DocumentTypes",
                column: "DocumentsEntityId",
                principalTable: "DocumentsEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
