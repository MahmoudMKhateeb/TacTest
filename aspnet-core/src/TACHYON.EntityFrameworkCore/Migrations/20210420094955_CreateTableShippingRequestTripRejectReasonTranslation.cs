using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CreateTableShippingRequestTripRejectReasonTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ShippingRequestReasonAccidents");

            migrationBuilder.CreateTable(
                name: "ShippingRequestReasonAccidentTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestReasonAccidentTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestReasonAccidentTranslations_ShippingRequestReasonAccidents_CoreId",
                        column: x => x.CoreId,
                        principalTable: "ShippingRequestReasonAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripRejectReasonTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripRejectReasonTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripRejectReasonTranslations_ShippingRequestTripRejectReasons_CoreId",
                        column: x => x.CoreId,
                        principalTable: "ShippingRequestTripRejectReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestReasonAccidentTranslations_CoreId",
                table: "ShippingRequestReasonAccidentTranslations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripRejectReasonTranslations_CoreId",
                table: "ShippingRequestTripRejectReasonTranslations",
                column: "CoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestReasonAccidentTranslations");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripRejectReasonTranslations");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ShippingRequestReasonAccidents",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }
    }
}
