using Microsoft.EntityFrameworkCore;
using BalloonShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BalloonShop.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Латексные", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Фольгированные", DisplayOrder = 2 },
            new Category { Id = 3, Name = "С рисунком", DisplayOrder = 3 }
        );

        // Seed stores
        modelBuilder.Entity<Store>().HasData(
            new Store { Id = 1, Name = "Шар №1 — Центр", Address = "ул. Ленина, 10" },
            new Store { Id = 2, Name = "Шар №2 — Север", Address = "пр. Победы, 45" }
        );

        // Seed employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, FullName = "Иванова Мария Петровна", Position = "Кассир", PaymentType = PaymentType.Salary, Rate = 35000, StoreId = 1 },
            new Employee { Id = 2, FullName = "Петров Алексей Иванович", Position = "Кассир", PaymentType = PaymentType.Percentage, Rate = 10, StoreId = 1 },
            new Employee { Id = 3, FullName = "Сидорова Елена Юрьевна", Position = "Старший кассир", PaymentType = PaymentType.Salary, Rate = 45000, StoreId = 2 }
        );

        // Seed products (with CategoryId)
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Шар красный латексный 12\"", CategoryId = 1, PurchasePrice = 15, SalePrice = 35, StockQuantity = 200, StoreId = 1 },
            new Product { Id = 2, Name = "Шар фольгированный Звезда", CategoryId = 2, PurchasePrice = 80, SalePrice = 180, StockQuantity = 50, StoreId = 1 },
            new Product { Id = 3, Name = "Шар с рисунком «С Днём Рождения»", CategoryId = 3, PurchasePrice = 60, SalePrice = 150, StockQuantity = 75, StoreId = 2 },
            new Product { Id = 4, Name = "Шар синий латексный 10\"", CategoryId = 1, PurchasePrice = 12, SalePrice = 30, StockQuantity = 300, StoreId = 2 },
            new Product { Id = 5, Name = "Шар фольгированный Сердце", CategoryId = 2, PurchasePrice = 90, SalePrice = 200, StockQuantity = 40, StoreId = 1 }
        );

        // Seed sales
        modelBuilder.Entity<Sale>().HasData(
            new Sale { Id = 1, SaleDate = DateTime.Today.AddDays(-20), ProductId = 1, Quantity = 10, EmployeeId = 1, StoreId = 1, TotalAmount = 350 },
            new Sale { Id = 2, SaleDate = DateTime.Today.AddDays(-15), ProductId = 2, Quantity = 3, EmployeeId = 2, StoreId = 1, TotalAmount = 540 },
            new Sale { Id = 3, SaleDate = DateTime.Today.AddDays(-10), ProductId = 3, Quantity = 5, EmployeeId = 3, StoreId = 2, TotalAmount = 750 },
            new Sale { Id = 4, SaleDate = DateTime.Today.AddDays(-5), ProductId = 5, Quantity = 2, EmployeeId = 1, StoreId = 1, TotalAmount = 400 },
            new Sale { Id = 5, SaleDate = DateTime.Today.AddDays(-3), ProductId = 4, Quantity = 20, EmployeeId = 3, StoreId = 2, TotalAmount = 600 },
            new Sale { Id = 6, SaleDate = DateTime.Today.AddDays(-1), ProductId = 1, Quantity = 15, EmployeeId = 2, StoreId = 1, TotalAmount = 525 }
        );

        // Configure AppUser - Employee relationship
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
