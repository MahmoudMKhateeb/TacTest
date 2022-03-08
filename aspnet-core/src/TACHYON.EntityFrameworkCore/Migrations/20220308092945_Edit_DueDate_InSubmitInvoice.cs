using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Edit_DueDate_InSubmitInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
 
            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "SubmitInvoices",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "SubmitInvoices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
