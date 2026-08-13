using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Data;

public class XeoTechDbContext : DbContext
{
    public XeoTechDbContext(DbContextOptions<XeoTechDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
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
    public DbSet<AppConfig> AppConfig => Set<AppConfig>();
    public DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
            .Property(p => p.Cost)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Customer>()
            .Property(c => c.CreditLimit)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .Property(o => o.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.Tax).HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.Shipping).HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.Total).HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.Discount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Quote)
            .WithMany()
            .HasForeignKey(o => o.QuoteId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Returns)
            .WithOne(r => r.Order)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithOne()
            .HasForeignKey<Invoice>(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseOrder>()
            .Property(p => p.Cost).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .Property(a => a.Cost).HasPrecision(18, 2);
        modelBuilder.Entity<Asset>()
            .Property(a => a.Salvage).HasPrecision(18, 2);

        modelBuilder.Entity<Budget>()
            .Property(b => b.MonthlyAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Budget>()
            .HasIndex(b => b.Category)
            .IsUnique();

        modelBuilder.Entity<AppConfig>()
            .HasIndex(a => a.Id);
    }
}