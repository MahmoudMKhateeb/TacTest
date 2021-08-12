using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Remove_ReceiverAddress_From_RoutPoint_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverAddress",
                table: "RoutPoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverAddress",
                table: "RoutPoints",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
