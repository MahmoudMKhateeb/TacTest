﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_ShippingRequestTripAccidentComment_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "ShippingRequestTripAccidentComments",
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
                    Comment = table.Column<string>(nullable: true),
                    AccidentId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripAccidentComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidentComments_ShippingRequestTripAccidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "ShippingRequestTripAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidentComments_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidentComments_AccidentId",
                table: "ShippingRequestTripAccidentComments",
                column: "AccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidentComments_TenantId",
                table: "ShippingRequestTripAccidentComments",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "ShippingRequestTripAccidentComments");


        }
    }
}