using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Data;

public class XeoTechDbContext(DbContextOptions<XeoTechDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<AppConfig> AppConfig => Set<AppConfig>();
    public DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<RefreshToken>().HasIndex(x => x.TokenHash).IsUnique();
        modelBuilder.Entity<RefreshToken>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Product>().Property(x => x.Price).HasPrecision(18,2);
        modelBuilder.Entity<Product>().Property(x => x.Cost).HasPrecision(18,2);
        modelBuilder.Entity<Product>().HasIndex(x => x.Sku).IsUnique();
        modelBuilder.Entity<Product>().HasOne(x => x.Supplier).WithMany(x => x.Products).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Customer>().Property(x => x.CreditLimit).HasPrecision(18,2);
        modelBuilder.Entity<Customer>().HasMany(x => x.Orders).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        foreach (var e in new[] { typeof(Order), typeof(Quote) }) { modelBuilder.Entity(e).Property("Subtotal").HasPrecision(18,2); modelBuilder.Entity(e).Property("Tax").HasPrecision(18,2); modelBuilder.Entity(e).Property("Shipping").HasPrecision(18,2); modelBuilder.Entity(e).Property("Total").HasPrecision(18,2); }
        modelBuilder.Entity<Order>().Property(x => x.Discount).HasPrecision(18,2);
        modelBuilder.Entity<Order>().HasOne(x => x.Quote).WithMany().HasForeignKey(x => x.QuoteId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Order>().HasMany(x => x.Payments).WithOne(x => x.Order).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Order>().HasMany(x => x.Returns).WithOne(x => x.Order).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Invoice>().Property(x => x.Amount).HasPrecision(18,2);
        modelBuilder.Entity<Invoice>().HasOne(x => x.Order).WithOne().HasForeignKey<Invoice>(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PurchaseOrder>().Property(x => x.Cost).HasPrecision(18,2);
        modelBuilder.Entity<PurchaseOrder>().HasOne(x => x.Supplier).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Asset>().Property(x => x.Cost).HasPrecision(18,2);
        modelBuilder.Entity<Asset>().Property(x => x.Salvage).HasPrecision(18,2);
        modelBuilder.Entity<Budget>().Property(x => x.MonthlyAmount).HasPrecision(18,2);
        modelBuilder.Entity<Budget>().HasIndex(x => x.Category).IsUnique();
        modelBuilder.Entity<Expense>().Property(x => x.Amount).HasPrecision(18,2);
    }
}
