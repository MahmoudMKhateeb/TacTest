using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removeTableTrancactionChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionsChannels_ChannelId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionsChannels");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ChannelId",
                table: "Transactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionsChannels",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsChannels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ChannelId",
                table: "Transactions",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionsChannels_ChannelId",
                table: "Transactions",
                column: "ChannelId",
                principalTable: "TransactionsChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
