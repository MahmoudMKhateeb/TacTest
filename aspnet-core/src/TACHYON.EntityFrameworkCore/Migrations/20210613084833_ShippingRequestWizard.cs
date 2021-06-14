using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestWizard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<byte>(
                name: "RouteTypeId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "PackingTypeId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OriginCityId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DestinationCityId",
                table: "ShippingRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsDrafted",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId",
                principalTable: "PackingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsDrafted",
                table: "ShippingRequests");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "RouteTypeId",
                table: "ShippingRequests",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PackingTypeId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OriginCityId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DestinationCityId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_DestinationCityId",
                table: "ShippingRequests",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Cities_OriginCityId",
                table: "ShippingRequests",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_PackingTypes_PackingTypeId",
                table: "ShippingRequests",
                column: "PackingTypeId",
                principalTable: "PackingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
