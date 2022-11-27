using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class phase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes");

            migrationBuilder.AddColumn<int>(
                name: "AppendixId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Commission",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TmsPricePackages",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "ProposalId",
                table: "PricePackageAppendixes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages",
                column: "AppendixId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes",
                column: "ProposalId",
                principalTable: "PricePackageProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_PricePackageAppendixes_AppendixId",
                table: "TmsPricePackages",
                column: "AppendixId",
                principalTable: "PricePackageAppendixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes");

            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_PricePackageAppendixes_AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TmsPricePackages");

            migrationBuilder.AlterColumn<int>(
                name: "ProposalId",
                table: "PricePackageAppendixes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageAppendixes_PricePackageProposals_ProposalId",
                table: "PricePackageAppendixes",
                column: "ProposalId",
                principalTable: "PricePackageProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
