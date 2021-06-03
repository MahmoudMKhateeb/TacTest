using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreateTableShippingRequestDirectRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "AbpUsers",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "AbpTenants",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingRequestDirectRequests",
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
                    CarrierTenantId = table.Column<int>(nullable: false),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    RejetcReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestDirectRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDirectRequests_AbpTenants_CarrierTenantId",
                        column: x => x.CarrierTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDirectRequests_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestDirectRequests_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestDirectRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_CarrierTenantId",
                table: "ShippingRequestDirectRequests",
                column: "CarrierTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_ShippingRequestId",
                table: "ShippingRequestDirectRequests",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_TenantId",
                table: "ShippingRequestDirectRequests",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequestDirectRequests_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestDirectRequestId",
                principalTable: "ShippingRequestDirectRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequestDirectRequests_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.DropTable(
                name: "ShippingRequestDirectRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "AbpUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "AbpTenants",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);
        }
    }
}
