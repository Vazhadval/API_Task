using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NaturalPersonAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    parentPersonId = table.Column<long>(type: "bigint", nullable: false),
                    RelatedPersonId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NaturalPeople",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturalPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NaturalPeople_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaturalPersonId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneNumbers_NaturalPeople_NaturalPersonId",
                        column: x => x.NaturalPersonId,
                        principalTable: "NaturalPeople",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CityName" },
                values: new object[,]
                {
                    { 1, "Asgard" },
                    { 2, "Gotham" },
                    { 3, "New York City" },
                    { 4, "Wakanda" },
                    { 5, "Kutaisi" }
                });

            migrationBuilder.InsertData(
                table: "Relations",
                columns: new[] { "Id", "RelatedPersonId", "RelationType", "parentPersonId" },
                values: new object[,]
                {
                    { 1L, 2L, "Friend", 1L },
                    { 2L, 3L, "Other", 1L },
                    { 3L, 3L, "Friend", 2L }
                });

            migrationBuilder.InsertData(
                table: "NaturalPeople",
                columns: new[] { "Id", "BirthDate", "CityId", "FirstName", "Gender", "LastName", "PersonalNumber", "Photo" },
                values: new object[,]
                {
                    { 4L, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Thor", "Male", "Odinson", "333", null },
                    { 3L, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Bruce", "Male", "Wayne", "333", null },
                    { 1L, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Pitter", "Male", "Parker", "111", null },
                    { 2L, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Black", "Male", "Panther", "222", null },
                    { 5L, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "glexo", "Male", "vichi", "333", null }
                });

            migrationBuilder.InsertData(
                table: "PhoneNumbers",
                columns: new[] { "Id", "NaturalPersonId", "Phone", "Type" },
                values: new object[,]
                {
                    { 1L, 1L, "555555111", "Home" },
                    { 2L, 1L, "555111666", "Office" },
                    { 3L, 2L, "555444666", "Home" },
                    { 4L, 2L, "555888777", "Office" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NaturalPeople_CityId",
                table: "NaturalPeople",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumbers_NaturalPersonId",
                table: "PhoneNumbers",
                column: "NaturalPersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneNumbers");

            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "NaturalPeople");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
