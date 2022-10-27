using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_2_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "PricePackageProposals",
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
                    ProposalName = table.Column<string>(nullable: true),
                    ScopeOverview = table.Column<string>(nullable: true),
                    ProposalDate = table.Column<DateTime>(nullable: true),
                    ShipperId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePackageProposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageProposals_AbpTenants_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TmsPricePackages",
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
                    PricePackageId = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false),
                    TrucksTypeId = table.Column<long>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    ShipperId = table.Column<int>(nullable: true),
                    RouteType = table.Column<byte>(nullable: false),
                    DirectRequestPrice = table.Column<decimal>(nullable: false),
                    TachyonManagePrice = table.Column<decimal>(nullable: false),
                    DirectRequestCommission = table.Column<decimal>(nullable: false),
                    TachyonManageCommission = table.Column<decimal>(nullable: false),
                    DirectRequestTotalPrice = table.Column<decimal>(nullable: false),
                    TachyonManageTotalPrice = table.Column<decimal>(nullable: false),
                    CommissionType = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ProposalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmsPricePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_PricePackageProposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "PricePackageProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_AbpTenants_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_TransportTypes_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalTable: "TransportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TmsPricePackages_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageProposals_ShipperId",
                table: "PricePackageProposals",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_DestinationCityId",
                table: "TmsPricePackages",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_OriginCityId",
                table: "TmsPricePackages",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ProposalId",
                table: "TmsPricePackages",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShipperId",
                table: "TmsPricePackages",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_TenantId",
                table: "TmsPricePackages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_TransportTypeId",
                table: "TmsPricePackages",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_TrucksTypeId",
                table: "TmsPricePackages",
                column: "TrucksTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TmsPricePackages");

            migrationBuilder.DropTable(
                name: "PricePackageProposals");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);
        }
    }
}
