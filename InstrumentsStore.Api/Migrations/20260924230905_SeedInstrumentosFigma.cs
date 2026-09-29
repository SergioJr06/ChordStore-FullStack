using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
// Esta migração é responsável por inserir dados iniciais para os instrumentos na base de dados.
// Ela cria a tabela "Instruments" com suas colunas e insere cinco registros de instrumentos musicais, cada um com informações como marca, categoria, descrição, preço, quantidade em estoque, entre outros.
// Além disso, cria um índice único na coluna "Slug" para garantir que cada instrumento tenha um identificador exclusivo.W

namespace InstrumentsStore.Api.Migrations
{
    
    public partial class SeedInstrumentosFigma : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder) // Este método é chamado quando a migração é aplicada. Ele cria a tabela "Instruments" e insere os dados iniciais.
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new // Define as colunas da tabela "Instruments"
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Slug = table.Column<string>(type: "varchar(255)", nullable: false) // Define a coluna "Slug" como uma string de até 255 caracteres, que não pode ser nula
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
                    table.PrimaryKey("PK_Instruments", x => x.Id); // Define a chave primária da tabela como a coluna "Id"
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData( // Insere dados iniciais na tabela "Instruments"
                table: "Instruments",
                columns: new[] { "Id", "Brand", "Category", "Description", "GalleryUrls", "ImageUrl", "Installments", "Name", "OldPrice", "Price", "Section", "Slug", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Fender", "Cordas", "Clássica e versátil.", "", "/images/guitarra.png", 12, "Guitarra Elétrica Stratocaster", null, 1800.00m, "novo", "guitarra", 15 },
                    { 2, "Yamaha", "Sopro", "Acabamento laqueado.", "", "/images/saxofone.png", 12, "Saxofone Alto", null, 3500.00m, "exclusivo", "saxofone", 4 },
                    { 3, "Roland", "Teclas", "61 teclas sensitivas.", "", "/images/teclado.png", 12, "Teclado Sintetizador", 2500.00m, 2200.00m, "novo", "teclado", 8 },
                    { 4, "Stradivarius", "Cordas", "Peça rara e restaurada.", "", "/images/violino.png", 12, "Violino Clássico", null, null, "exclusivo", "violino", 1 },
                    { 5, "Pearl", "Percussão", "Kit completo com pratos.", "", "/images/bateria.png", 12, "Bateria Acústica", 3500.00m, 2900.00m, "promocao", "bateria", 3 }
                }); // Insere cinco registros de instrumentos musicais com suas respectivas informações

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Slug",
                table: "Instruments",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) // Este método é chamado quando a migração é revertida. Ele remove a tabela "Instruments" do banco de dados.
        {
            migrationBuilder.DropTable(
                name: "Instruments");
        }
    }
}
