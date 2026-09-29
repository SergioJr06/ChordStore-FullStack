using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
// codigo gerado automaticamente para a migração, não deve ser alterado manualmente
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InstrumentsStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Instruments");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Instruments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Instruments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Document = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Document = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CustomerNameSnapshot = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerEmailSnapshot = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerPhoneSnapshot = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerAddressSnapshot = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Installments = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Origin = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SaleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleItems_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AdminUsers",
                columns: new[] { "Id", "CreatedAt", "Email", "LastLoginAt", "PasswordHash", "Username" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "contato@chordstore.com", null, "100000.0iOmMqXGLxiV2B6nKB6njw==.rI4jbbCGMCeN3HanEUsapojoWm0zfKr2xRx8Hl2q3/o=", "admin" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Violões, guitarras, baixos e violinos", "Cordas" },
                    { 2, "Saxofones, trompetes e afins", "Sopro" },
                    { 3, "Teclados, pianos e sintetizadores", "Teclas" },
                    { 4, "Baterias e instrumentos de percussão", "Percussão" }
                });

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "SupplierId" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "SupplierId" },
                values: new object[] { 2, 2 });

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "SupplierId" },
                values: new object[] { 3, 3 });

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "SupplierId" },
                values: new object[] { 1, 4 });

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "SupplierId" },
                values: new object[] { 4, 5 });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "Document", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, null, null, "vendas@fender.com.br", "Fender Brasil", "(11) 4000-0001" },
                    { 2, null, null, "vendas@yamaha.com.br", "Yamaha Brasil", "(11) 4000-0002" },
                    { 3, null, null, "vendas@roland.com.br", "Roland Brasil", "(11) 4000-0003" },
                    { 4, null, null, "contato@stradivarius-imports.com", "Stradivarius Imports", "(11) 4000-0004" },
                    { 5, null, null, "vendas@pearldrums.com.br", "Pearl Drums Brasil", "(11) 4000-0005" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_CategoryId",
                table: "Instruments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_SupplierId",
                table: "Instruments",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_Email",
                table: "AdminUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_Username",
                table: "AdminUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_InstrumentId",
                table: "SaleItems",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instruments_Categories_CategoryId",
                table: "Instruments",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Instruments_Suppliers_SupplierId",
                table: "Instruments",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instruments_Categories_CategoryId",
                table: "Instruments");

            migrationBuilder.DropForeignKey(
                name: "FK_Instruments_Suppliers_SupplierId",
                table: "Instruments");

            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "SaleItems");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Instruments_CategoryId",
                table: "Instruments");

            migrationBuilder.DropIndex(
                name: "IX_Instruments_SupplierId",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Instruments");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Instruments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: "Cordas");

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: "Sopro");

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: "Teclas");

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: "Cordas");

            migrationBuilder.UpdateData(
                table: "Instruments",
                keyColumn: "Id",
                keyValue: 5,
                column: "Category",
                value: "Percussão");
        }
    }
}
