using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class groupperiodinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClaims",
                table: "GroupPeriods");

            migrationBuilder.AddColumn<bool>(
                name: "IsClaim",
                table: "GroupPeriods",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GroupPeriodsInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(nullable: false),
                    GroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPeriodsInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPeriodsInvoices_GroupPeriods_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPeriodsInvoices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriodsInvoices_GroupId",
                table: "GroupPeriodsInvoices",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPeriodsInvoices_InvoiceId",
                table: "GroupPeriodsInvoices",
                column: "InvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupPeriodsInvoices");

            migrationBuilder.DropColumn(
                name: "IsClaim",
                table: "GroupPeriods");

            migrationBuilder.AddColumn<bool>(
                name: "IsClaims",
                table: "GroupPeriods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
