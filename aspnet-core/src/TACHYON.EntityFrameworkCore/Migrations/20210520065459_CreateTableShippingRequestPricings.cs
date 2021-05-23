using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreateTableShippingRequestPricings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRequestPricings",
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
                    ParentId = table.Column<long>(nullable: true),
                    ShippingRequest = table.Column<long>(nullable: false),
                    SourceId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    Channel = table.Column<byte>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TripPrice = table.Column<decimal>(nullable: false),
                    TripVatAmount = table.Column<decimal>(nullable: false),
                    TripTotalAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    SubTotalAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    TaxVat = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<byte>(nullable: false),
                    CommissionValue = table.Column<decimal>(nullable: false),
                    CommissionPercentage = table.Column<decimal>(nullable: false),
                    Commission = table.Column<decimal>(nullable: false),
                    RejectedReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPricings_ShippingRequestPricings_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ShippingRequestPricings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                        column: x => x.ShippingRequest,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestPricings_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ParentId",
                table: "ShippingRequestPricings",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ShippingRequest",
                table: "ShippingRequestPricings",
                column: "ShippingRequest");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_TenantId",
                table: "ShippingRequestPricings",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestPricings");
        }
    }
}
