using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PricePackageAppendixes",
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
                    ContractName = table.Column<string>(nullable: true),
                    ContractNumber = table.Column<int>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    AppendixDate = table.Column<DateTime>(nullable: true),
                    ScopeOverview = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    ProposalId = table.Column<int>(nullable: false),
                    AppendixFileId = table.Column<Guid>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePackageAppendixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "PricePackageProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageAppendixes_ProposalId",
                table: "PricePackageAppendixes",
                column: "ProposalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TmsPricePackages");
        }
    }
}
