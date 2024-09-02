using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _2281 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicInvoiceCustomItems",
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
                    DynamicInvoiceId = table.Column<long>(nullable: false),
                    ItemName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    VatTax = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicInvoiceCustomItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicInvoiceCustomItems_DynamicInvoices_DynamicInvoiceId",
                        column: x => x.DynamicInvoiceId,
                        principalTable: "DynamicInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicInvoiceCustomItems_DynamicInvoiceId",
                table: "DynamicInvoiceCustomItems",
                column: "DynamicInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicInvoiceCustomItems");
        }
    }
}
