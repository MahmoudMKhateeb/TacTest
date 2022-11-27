using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Remove_Name_From_DriverLicenseType_And_DriverLicenseTypeTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Name",
            //    table: "DriverLicenseTypeTranslations");

            //migrationBuilder.DropColumn(
            //    name: "Name",
            //    table: "DriverLicenseTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "DriverLicenseTypeTranslations",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "DriverLicenseTypes",
            //    type: "nvarchar(max)",
            //    nullable: true);
        }
    }
}
