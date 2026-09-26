using System;
using InstrumentsStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Instrument> Instruments { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<AdminUser> AdminUsers { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;

    // Data fixa (sem DateTime.UtcNow) pra manter o seed determinístico entre migrations.
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Instrument ----------
        modelBuilder.Entity<Instrument>()
            .HasIndex(i => i.Slug)
            .IsUnique();

        modelBuilder.Entity<Instrument>()
            .Property(i => i.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Instrument>()
            .Property(i => i.OldPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Instrument>()
            .HasOne(i => i.Category)
            .WithMany(c => c.Instruments)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Instrument>()
            .HasOne(i => i.Supplier)
            .WithMany(s => s.Instruments)
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        // ---------- Sale / SaleItem ----------
        modelBuilder.Entity<Sale>()
            .Property(s => s.Total)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SaleItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<SaleItem>()
            .Property(i => i.Subtotal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<SaleItem>()
            .HasOne(i => i.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaleItem>()
            .HasOne(i => i.Instrument)
            .WithMany()
            .HasForeignKey(i => i.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- AdminUser ----------
        modelBuilder.Entity<AdminUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<AdminUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // =====================================================================
        // SEED (dados iniciais)
        // =====================================================================

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Cordas", Description = "Violões, guitarras, baixos e violinos" },
            new Category { Id = 2, Name = "Sopro", Description = "Saxofones, trompetes e afins" },
            new Category { Id = 3, Name = "Teclas", Description = "Teclados, pianos e sintetizadores" },
            new Category { Id = 4, Name = "Percussão", Description = "Baterias e instrumentos de percussão" }
        );

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "Fender Brasil", Email = "vendas@fender.com.br", Phone = "(11) 4000-0001" },
            new Supplier { Id = 2, Name = "Yamaha Brasil", Email = "vendas@yamaha.com.br", Phone = "(11) 4000-0002" },
            new Supplier { Id = 3, Name = "Roland Brasil", Email = "vendas@roland.com.br", Phone = "(11) 4000-0003" },
            new Supplier { Id = 4, Name = "Stradivarius Imports", Email = "contato@stradivarius-imports.com", Phone = "(11) 4000-0004" },
            new Supplier { Id = 5, Name = "Pearl Drums Brasil", Email = "vendas@pearldrums.com.br", Phone = "(11) 4000-0005" }
        );

        modelBuilder.Entity<Instrument>().HasData(
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
            new Instrument
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
            new Instrument
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
            new Instrument
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
            new Instrument
            {
                Id = 5,
                Slug = "bateria",
                Name = "Bateria Acústica",
                Section = "promocao",
                Price = 2900.00m,
                OldPrice = 3500.00m,
                Installments = 12, // Regra de R$ 2.900 aplicada
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
        // (troque a senha na tela "Minha Conta" do admin assim que possível)
        modelBuilder.Entity<AdminUser>().HasData(
            new AdminUser
            {
                Id = 1,
                Username = "admin",
                Email = "contato@chordstore.com",
                PasswordHash = "100000.0iOmMqXGLxiV2B6nKB6njw==.rI4jbbCGMCeN3HanEUsapojoWm0zfKr2xRx8Hl2q3/o=",
                CreatedAt = SeedDate
            }
        );
    }
}
