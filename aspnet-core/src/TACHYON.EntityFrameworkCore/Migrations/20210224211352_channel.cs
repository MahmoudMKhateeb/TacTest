using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class channel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TrancactionChannels_ChannelId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TrancactionChannels");

            migrationBuilder.CreateTable(
                name: "TransactionsChannels",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Channel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsChannels", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionsChannels_ChannelId",
                table: "Transactions",
                column: "ChannelId",
                principalTable: "TransactionsChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionsChannels_ChannelId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionsChannels");

            migrationBuilder.CreateTable(
                name: "TrancactionChannels",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrancactionChannels", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TrancactionChannels_ChannelId",
                table: "Transactions",
                column: "ChannelId",
                principalTable: "TrancactionChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
