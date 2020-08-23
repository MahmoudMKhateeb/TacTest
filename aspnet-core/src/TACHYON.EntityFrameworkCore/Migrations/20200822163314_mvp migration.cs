using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class mvpmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DrivingLicenseExpiryDate",
                table: "AbpUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DrivingLicenseIssuingDate",
                table: "AbpUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseNumber",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceField",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDriver",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Code = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
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
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    HasExpirationDate = table.Column<bool>(nullable: false),
                    RequiredFrom = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayloadMaxWeights",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false),
                    MaxWeight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayloadMaxWeights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrailerStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrailerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrailerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrailerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TruckStatuses",
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
                    TenantId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrucksTypes",
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
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrucksTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    Latitude = table.Column<string>(maxLength: 256, nullable: true),
                    Longitude = table.Column<string>(maxLength: 256, nullable: true),
                    CountyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsDetails",
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
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Quantity = table.Column<string>(maxLength: 128, nullable: true),
                    Weight = table.Column<string>(maxLength: 64, nullable: true),
                    Dimentions = table.Column<string>(maxLength: 128, nullable: true),
                    IsDangerousGood = table.Column<bool>(nullable: false),
                    DangerousGoodsCode = table.Column<string>(maxLength: 64, nullable: true),
                    GoodCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                        column: x => x.GoodCategoryId,
                        principalTable: "GoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    RoutTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_RoutTypes_RoutTypeId",
                        column: x => x.RoutTypeId,
                        principalTable: "RoutTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PlateNumber = table.Column<string>(maxLength: 64, nullable: false),
                    ModelName = table.Column<string>(maxLength: 64, nullable: false),
                    ModelYear = table.Column<string>(maxLength: 64, nullable: false),
                    LicenseNumber = table.Column<string>(maxLength: 256, nullable: false),
                    LicenseExpirationDate = table.Column<DateTime>(nullable: false),
                    IsAttachable = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(maxLength: 256, nullable: true),
                    TrucksTypeId = table.Column<long>(nullable: false),
                    TruckStatusId = table.Column<long>(nullable: false),
                    Driver1UserId = table.Column<long>(nullable: true),
                    Driver2UserId = table.Column<long>(nullable: true),
                    RentPrice = table.Column<int>(nullable: true),
                    RentDuration = table.Column<int>(nullable: true),
                    PictureId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_AbpUsers_Driver1UserId",
                        column: x => x.Driver1UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trucks_AbpUsers_Driver2UserId",
                        column: x => x.Driver2UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trucks_TruckStatuses_TruckStatusId",
                        column: x => x.TruckStatusId,
                        principalTable: "TruckStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trucks_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    TrucksTypeId = table.Column<long>(nullable: false),
                    TrailerTypeId = table.Column<int>(nullable: false),
                    GoodCategoryId = table.Column<int>(nullable: true),
                    RouteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_GoodCategories_GoodCategoryId",
                        column: x => x.GoodCategoryId,
                        principalTable: "GoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offers_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_TrailerTypes_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalTable: "TrailerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequests",
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
                    TenantId = table.Column<int>(nullable: false),
                    Vas = table.Column<decimal>(nullable: false),
                    TrucksTypeId = table.Column<long>(nullable: true),
                    TrailerTypeId = table.Column<int>(nullable: true),
                    GoodsDetailId = table.Column<long>(nullable: true),
                    RouteId = table.Column<int>(nullable: true),
                    IsBid = table.Column<bool>(nullable: false),
                    IsTachyonDeal = table.Column<bool>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    IsPriceAccepted = table.Column<bool>(nullable: true),
                    IsRejected = table.Column<bool>(nullable: true),
                    FatherShippingRequestId = table.Column<long>(nullable: true),
                    CarrierTenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_AbpTenants_CarrierTenantId",
                        column: x => x.CarrierTenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_ShippingRequests_FatherShippingRequestId",
                        column: x => x.FatherShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_GoodsDetails_GoodsDetailId",
                        column: x => x.GoodsDetailId,
                        principalTable: "GoodsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalTable: "TrailerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                        column: x => x.TrucksTypeId,
                        principalTable: "TrucksTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
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
                    TenantId = table.Column<int>(nullable: false),
                    TrailerCode = table.Column<string>(maxLength: 256, nullable: false),
                    PlateNumber = table.Column<string>(maxLength: 256, nullable: false),
                    Model = table.Column<string>(maxLength: 64, nullable: false),
                    Year = table.Column<string>(maxLength: 64, nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    IsLiftgate = table.Column<bool>(nullable: false),
                    IsReefer = table.Column<bool>(nullable: false),
                    IsVented = table.Column<bool>(nullable: false),
                    IsRollDoor = table.Column<bool>(nullable: false),
                    TrailerStatusId = table.Column<int>(nullable: false),
                    TrailerTypeId = table.Column<int>(nullable: false),
                    PayloadMaxWeightId = table.Column<int>(nullable: false),
                    HookedTruckId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trailers_Trucks_HookedTruckId",
                        column: x => x.HookedTruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trailers_PayloadMaxWeights_PayloadMaxWeightId",
                        column: x => x.PayloadMaxWeightId,
                        principalTable: "PayloadMaxWeights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trailers_TrailerStatuses_TrailerStatusId",
                        column: x => x.TrailerStatusId,
                        principalTable: "TrailerStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trailers_TrailerTypes_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalTable: "TrailerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutSteps",
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
                    TenantId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    Latitude = table.Column<string>(maxLength: 256, nullable: true),
                    Longitude = table.Column<string>(maxLength: 256, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    OriginCityId = table.Column<int>(nullable: true),
                    DestinationCityId = table.Column<int>(nullable: true),
                    RouteId = table.Column<int>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutSteps_Cities_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutSteps_Cities_OriginCityId",
                        column: x => x.OriginCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutSteps_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Extn = table.Column<string>(maxLength: 100, nullable: false),
                    BinaryObjectId = table.Column<Guid>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    IsAccepted = table.Column<string>(nullable: true),
                    DocumentTypeId = table.Column<long>(nullable: false),
                    TruckId = table.Column<Guid>(nullable: true),
                    TrailerId = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    RoutStepId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_RoutSteps_RoutStepId",
                        column: x => x.RoutStepId,
                        principalTable: "RoutSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_Trailers_TrailerId",
                        column: x => x.TrailerId,
                        principalTable: "Trailers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountyId",
                table: "Cities",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_DocumentTypeId",
                table: "DocumentFiles",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_RoutStepId",
                table: "DocumentFiles",
                column: "RoutStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_TenantId",
                table: "DocumentFiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_TrailerId",
                table: "DocumentFiles",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_TruckId",
                table: "DocumentFiles",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_UserId",
                table: "DocumentFiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_GoodCategoryId",
                table: "Offers",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_RouteId",
                table: "Offers",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TenantId",
                table: "Offers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TrailerTypeId",
                table: "Offers",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TrucksTypeId",
                table: "Offers",
                column: "TrucksTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RoutTypeId",
                table: "Routes",
                column: "RoutTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_TenantId",
                table: "Routes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_DestinationCityId",
                table: "RoutSteps",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_OriginCityId",
                table: "RoutSteps",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_RouteId",
                table: "RoutSteps",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TenantId",
                table: "RoutSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_CarrierTenantId",
                table: "ShippingRequests",
                column: "CarrierTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_FatherShippingRequestId",
                table: "ShippingRequests",
                column: "FatherShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_GoodsDetailId",
                table: "ShippingRequests",
                column: "GoodsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_RouteId",
                table: "ShippingRequests",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TenantId",
                table: "ShippingRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_HookedTruckId",
                table: "Trailers",
                column: "HookedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_PayloadMaxWeightId",
                table: "Trailers",
                column: "PayloadMaxWeightId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_TenantId",
                table: "Trailers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_TrailerStatusId",
                table: "Trailers",
                column: "TrailerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_TrailerTypeId",
                table: "Trailers",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Driver1UserId",
                table: "Trucks",
                column: "Driver1UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Driver2UserId",
                table: "Trucks",
                column: "Driver2UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TenantId",
                table: "Trucks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckStatusId",
                table: "Trucks",
                column: "TruckStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TrucksTypeId",
                table: "Trucks",
                column: "TrucksTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckStatuses_TenantId",
                table: "TruckStatuses",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentFiles");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "RoutSteps");

            migrationBuilder.DropTable(
                name: "Trailers");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ShippingRequests");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "PayloadMaxWeights");

            migrationBuilder.DropTable(
                name: "TrailerStatuses");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "GoodsDetails");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "TrailerTypes");

            migrationBuilder.DropTable(
                name: "TruckStatuses");

            migrationBuilder.DropTable(
                name: "TrucksTypes");

            migrationBuilder.DropTable(
                name: "GoodCategories");

            migrationBuilder.DropTable(
                name: "RoutTypes");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseExpiryDate",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseIssuingDate",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseNumber",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ExperienceField",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsDriver",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AbpUsers");
        }
    }
}
