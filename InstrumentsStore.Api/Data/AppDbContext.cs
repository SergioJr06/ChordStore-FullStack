using System;
using InstrumentsStore.Api.Models;
using Microsoft.EntityFrameworkCore;

// O código define o contexto do Entity Framework Core (AppDbContext) para a aplicação InstrumentsStore.Api. Ele herda de DbContext e configura os DbSets para as entidades do domínio, como Instrument, Category, Supplier, Customer, AdminUser, Sale e SaleItem. Além disso, ele define regras de mapeamento e relacionamentos entre as entidades, incluindo restrições de integridade referencial e precisão de campos monetários.
// O método OnModelCreating também inclui dados iniciais
// (seed data) para popular o banco de dados com categorias, fornecedores, instrumentos e um usuário administrador padrão.

namespace InstrumentsStore.Api.Data;

public class AppDbContext : DbContext // Contexto do Entity Framework Core para a aplicação
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Instrument> Instruments { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<AdminUser> AdminUsers { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!; // DbSet para itens de venda (SaleItem)

  
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    // O método OnModelCreating é sobrescrito para configurar o modelo de dados usando a API Fluent do Entity Framework Core. Ele define índices únicos, precisão de campos monetários, relacionamentos entre entidades e regras de exclusão em cascata ou restrição. Além disso,
    // ele insere dados iniciais (seed data) para categorias, fornecedores, instrumentos e um usuário administrador padrão.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Chamada ao método base para garantir que a configuração padrão do EF Core seja aplicada.

        // ---------- Instrumentos ----------
        modelBuilder.Entity<Instrument>() // Configura a entidade Instrument
            .HasIndex(i => i.Slug)
            .IsUnique();

        modelBuilder.Entity<Instrument>() // Configura a precisão do campo Price para 10 dígitos no total, com 2 casas decimais.
            .Property(i => i.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Instrument>() // Configura a precisão do campo OldPrice para 10 dígitos no total, com 2 casas decimais.
            .Property(i => i.OldPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Instrument>() // Configura o relacionamento entre Instrument e Category, definindo que um instrumento pertence a uma categoria e que a exclusão de uma categoria não deve excluir os instrumentos associados (SetNull).
            .HasOne(i => i.Category)
            .WithMany(c => c.Instruments)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Instrument>() // Configura o relacionamento entre Instrument e Supplier, definindo que um instrumento pertence a um fornecedor e que a exclusão de um fornecedor não deve excluir os instrumentos associados (SetNull).
            .HasOne(i => i.Supplier)
            .WithMany(s => s.Instruments)
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        // ---------- Sale / SaleItem ----------
        modelBuilder.Entity<Sale>() // Configura a precisão do campo Total para 10 dígitos no total, com 2 casas decimais.
            .Property(s => s.Total)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Sale>() // Configura o relacionamento entre Sale e Customer, definindo que uma venda pertence a um cliente e que a exclusão de um cliente não deve excluir as vendas associadas (SetNull).
            .HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SaleItem>() // Configura a precisão do campo UnitPrice para 10 dígitos no total, com 2 casas decimais.
            .Property(i => i.UnitPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<SaleItem>() // Configura a precisão do campo Subtotal para 10 dígitos no total, com 2 casas decimais.
            .Property(i => i.Subtotal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<SaleItem>() // Configura o relacionamento entre SaleItem e Sale, definindo que um item de venda pertence a uma venda e que a exclusão de uma venda deve excluir os itens associados (Cascade).
            .HasOne(i => i.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaleItem>() // Configura o relacionamento entre SaleItem e Instrument, definindo que um item de venda pertence a um instrumento e que a exclusão de um instrumento não deve excluir os itens de venda associados (Restrict).
            .HasOne(i => i.Instrument)
            .WithMany()
            .HasForeignKey(i => i.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- AdminUser ----------
        modelBuilder.Entity<AdminUser>() // Configura índices únicos para os campos Username e Email da entidade AdminUser, garantindo que não haja duplicatas no banco de dados.
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<AdminUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ---------- Seed Data ---------- 
        // Insere dados iniciais (seed data) para popular o banco de dados com categorias, fornecedores, instrumentos e um usuário administrador padrão.

        modelBuilder.Entity<Category>().HasData( // Insere dados iniciais para a entidade Category
            new Category { Id = 1, Name = "Cordas", Description = "Violões, guitarras, baixos e violinos" },
            new Category { Id = 2, Name = "Sopro", Description = "Saxofones, trompetes e afins" },
            new Category { Id = 3, Name = "Teclas", Description = "Teclados, pianos e sintetizadores" },
            new Category { Id = 4, Name = "Percussão", Description = "Baterias e instrumentos de percussão" }
        );

        modelBuilder.Entity<Supplier>().HasData( // Insere dados iniciais para a entidade Supplier
            new Supplier { Id = 1, Name = "Fender Brasil", Email = "vendas@fender.com.br", Phone = "(11) 4000-0001" },
            new Supplier { Id = 2, Name = "Yamaha Brasil", Email = "vendas@yamaha.com.br", Phone = "(11) 4000-0002" },
            new Supplier { Id = 3, Name = "Roland Brasil", Email = "vendas@roland.com.br", Phone = "(11) 4000-0003" },
            new Supplier { Id = 4, Name = "Stradivarius Imports", Email = "contato@stradivarius-imports.com", Phone = "(11) 4000-0004" },
            new Supplier { Id = 5, Name = "Pearl Drums Brasil", Email = "vendas@pearldrums.com.br", Phone = "(11) 4000-0005" }
        );

        modelBuilder.Entity<Instrument>().HasData( // Insere dados iniciais para a entidade Instrument
            new Instrument
            {
                Id = 1,
                Slug = "guitarra",
                Name = "Guitarra Elétrica Stratocaster",
                Section = "novo",
                Price = 1800.00m,
                OldPrice = null,
                Installments = 12,
                Description = "Clássica e versátil.",
                ImageUrl = "/images/guitarra.png",
                GalleryUrls = "",
                Brand = "Fender",
                StockQuantity = 15,
                CategoryId = 1,
                SupplierId = 1
            },
            new Instrument // Insere outro instrumento com preço sob consulta (null) e estoque limitado
            {
                Id = 2,
                Slug = "saxofone",
                Name = "Saxofone Alto",
                Section = "exclusivo",
                Price = 3500.00m,
                OldPrice = null,
                Installments = 12,
                Description = "Acabamento laqueado.",
                ImageUrl = "/images/saxofone.png",
                GalleryUrls = "",
                Brand = "Yamaha",
                StockQuantity = 4,
                CategoryId = 2,
                SupplierId = 2
            },
            new Instrument // Insere outro instrumento com preço promocional e estoque limitado
            {
                Id = 3,
                Slug = "teclado",
                Name = "Teclado Sintetizador",
                Section = "novo",
                Price = 2200.00m,
                OldPrice = 2500.00m,
                Installments = 12,
                Description = "61 teclas sensitivas.",
                ImageUrl = "/images/teclado.png",
                GalleryUrls = "",
                Brand = "Roland",
                StockQuantity = 8,
                CategoryId = 3,
                SupplierId = 3
            },
            new Instrument // Insere outro instrumento com preço sob consulta (null) e estoque limitado
            {
                Id = 4,
                Slug = "violino",
                Name = "Violino Clássico",
                Section = "exclusivo",
                Price = null,
                OldPrice = null,
                Installments = 12, // Preço sob consulta (null)
                Description = "Peça rara e restaurada.",
                ImageUrl = "/images/violino.png",
                GalleryUrls = "",
                Brand = "Stradivarius",
                StockQuantity = 1,
                CategoryId = 1,
                SupplierId = 4
            },
            new Instrument // Insere outro instrumento com preço promocional e estoque limitado
            {
                Id = 5,
                Slug = "bateria",
                Name = "Bateria Acústica",
                Section = "promocao",
                Price = 2900.00m,
                OldPrice = 3500.00m,
                Installments = 12,
                Description = "Kit completo com pratos.",
                ImageUrl = "/images/bateria.png",
                GalleryUrls = "",
                Brand = "Pearl",
                StockQuantity = 3,
                CategoryId = 4,
                SupplierId = 5
            }
        );

        // Usuário admin padrão -> login: contato@chordstore.com / senha: admin123
        modelBuilder.Entity<AdminUser>().HasData( // Insere um usuário administrador padrão com senha hash
            new AdminUser
            {
                Id = 1,
                Username = "admin",
                Email = "contato@chordstore.com",
                PasswordHash = "100000.0iOmMqXGLxiV2B6nKB6njw==.rI4jbbCGMCeN3HanEUsapojoWm0zfKr2xRx8Hl2q3/o=",
                CreatedAt = SeedDate // Define a data de criação do usuário administrador padrão como a data de seed (1º de janeiro de 2026)
            }
        );
    }
}
