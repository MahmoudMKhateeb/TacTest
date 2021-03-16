using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DemandFileExtn",
                table: "GroupPeriods");

            migrationBuilder.CreateTable(
                name: "TrancactionChannels",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Channel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrancactionChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<byte>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    SourceId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_TrancactionChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TrancactionChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ChannelId",
                table: "Transactions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TrancactionChannels");

            migrationBuilder.AddColumn<string>(
                name: "DemandFileExtn",
                table: "GroupPeriods",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
