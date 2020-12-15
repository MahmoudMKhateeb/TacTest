using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class changepricesinshippingrequestvastodouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DefualtPrice",
                table: "ShippingRequestVases",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ActualPrice",
                table: "ShippingRequestVases",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DefualtPrice",
                table: "ShippingRequestVases",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ActualPrice",
                table: "ShippingRequestVases",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
