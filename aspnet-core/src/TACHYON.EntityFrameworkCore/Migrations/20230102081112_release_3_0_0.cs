using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_3_0_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_UnitOfMeasures_UnitOfMeasureId",
                table: "GoodsDetails");

            migrationBuilder.AddColumn<byte>(
                name: "ShippingRequestTripFlag",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "DropPaymentMethod",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryConfiremed",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsPOD",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsReceiverCode",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PricePackageOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PricePackageOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PricePackageOfferItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PricePackageOfferItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ports",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Ports",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfMeasureId",
                table: "GoodsDetails",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "GoodsDetails",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Facilities",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Facilities",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<long>(
                name: "ActorInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ActorSubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorInvoiceChannel",
                table: "ActorSubmitInvoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActorInvoiceChannel",
                table: "ActorInvoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DedicatedDynamicActorInvoices",
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
                    TenantId = table.Column<int>(nullable: false),
                    ShipperActorId = table.Column<int>(nullable: true),
                    CarrierActorId = table.Column<int>(nullable: true),
                    InvoiceAccountType = table.Column<byte>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    ActorInvoiceId = table.Column<long>(nullable: true),
                    ActorSubmitInvoiceId = table.Column<long>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedDynamicActorInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoices_ActorInvoices_ActorInvoiceId",
                        column: x => x.ActorInvoiceId,
                        principalTable: "ActorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoices_ActorSubmitInvoices_ActorSubmitInvoiceId",
                        column: x => x.ActorSubmitInvoiceId,
                        principalTable: "ActorSubmitInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoices_Actors_CarrierActorId",
                        column: x => x.CarrierActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoices_Actors_ShipperActorId",
                        column: x => x.ShipperActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoices_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoices_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedDynamicActorInvoiceItems",
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
                    DedicatedDynamicActorInvoiceId = table.Column<long>(nullable: false),
                    DedicatedShippingRequestTruckId = table.Column<long>(nullable: false),
                    NumberOfDays = table.Column<int>(nullable: false),
                    PricePerDay = table.Column<decimal>(nullable: false),
                    AllNumberDays = table.Column<int>(nullable: false),
                    WorkingDayType = table.Column<byte>(nullable: false),
                    ItemSubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    ItemTotalAmount = table.Column<decimal>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedDynamicActorInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoiceItems_DedicatedDynamicActorInvoices_DedicatedDynamicActorInvoiceId",
                        column: x => x.DedicatedDynamicActorInvoiceId,
                        principalTable: "DedicatedDynamicActorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedDynamicActorInvoiceItems_DedicatedShippingRequestTrucks_DedicatedShippingRequestTruckId",
                        column: x => x.DedicatedShippingRequestTruckId,
                        principalTable: "DedicatedShippingRequestTrucks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_ActorInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "ActorInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_ActorSubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "ActorSubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoiceItems_DedicatedDynamicActorInvoiceId",
                table: "DedicatedDynamicActorInvoiceItems",
                column: "DedicatedDynamicActorInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoiceItems_DedicatedShippingRequestTruckId",
                table: "DedicatedDynamicActorInvoiceItems",
                column: "DedicatedShippingRequestTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoices_ActorInvoiceId",
                table: "DedicatedDynamicActorInvoices",
                column: "ActorInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoices_ActorSubmitInvoiceId",
                table: "DedicatedDynamicActorInvoices",
                column: "ActorSubmitInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoices_CarrierActorId",
                table: "DedicatedDynamicActorInvoices",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoices_ShipperActorId",
                table: "DedicatedDynamicActorInvoices",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoices_ShippingRequestId",
                table: "DedicatedDynamicActorInvoices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedDynamicActorInvoices_TenantId",
                table: "DedicatedDynamicActorInvoices",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_ActorInvoices_ActorInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "ActorInvoiceId",
                principalTable: "ActorInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_ActorSubmitInvoices_ActorSubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks",
                column: "ActorSubmitInvoiceId",
                principalTable: "ActorSubmitInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_UnitOfMeasures_UnitOfMeasureId",
                table: "GoodsDetails",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_ActorInvoices_ActorInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_ActorSubmitInvoices_ActorSubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_UnitOfMeasures_UnitOfMeasureId",
                table: "GoodsDetails");

            migrationBuilder.DropTable(
                name: "DedicatedDynamicActorInvoiceItems");

            migrationBuilder.DropTable(
                name: "DedicatedDynamicActorInvoices");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_ActorInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_ActorSubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripFlag",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "DropPaymentMethod",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsDeliveryConfiremed",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NeedsPOD",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NeedsReceiverCode",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PricePackageOfferItems");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PricePackageOfferItems");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForMultipleDrop",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValueForSingleDrop",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ActorInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ActorSubmitInvoiceId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ActorInvoiceChannel",
                table: "ActorSubmitInvoices");

            migrationBuilder.DropColumn(
                name: "ActorInvoiceChannel",
                table: "ActorInvoices");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Ports",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfMeasureId",
                table: "GoodsDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "GoodsDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Facilities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Facilities",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_UnitOfMeasures_UnitOfMeasureId",
                table: "GoodsDetails",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
