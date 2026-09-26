using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InstrumentsStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInstrumentosFigma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Slug = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Section = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    OldPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Installments = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GalleryUrls = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Brand = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Instruments",
                columns: new[] { "Id", "Brand", "Category", "Description", "GalleryUrls", "ImageUrl", "Installments", "Name", "OldPrice", "Price", "Section", "Slug", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Fender", "Cordas", "Clássica e versátil.", "", "/images/guitarra.png", 12, "Guitarra Elétrica Stratocaster", null, 1800.00m, "novo", "guitarra", 15 },
                    { 2, "Yamaha", "Sopro", "Acabamento laqueado.", "", "/images/saxofone.png", 12, "Saxofone Alto", null, 3500.00m, "exclusivo", "saxofone", 4 },
                    { 3, "Roland", "Teclas", "61 teclas sensitivas.", "", "/images/teclado.png", 12, "Teclado Sintetizador", 2500.00m, 2200.00m, "novo", "teclado", 8 },
                    { 4, "Stradivarius", "Cordas", "Peça rara e restaurada.", "", "/images/violino.png", 12, "Violino Clássico", null, null, "exclusivo", "violino", 1 },
                    { 5, "Pearl", "Percussão", "Kit completo com pratos.", "", "/images/bateria.png", 12, "Bateria Acústica", 3500.00m, 2900.00m, "promocao", "bateria", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Slug",
                table: "Instruments",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instruments");
        }
    }
}
