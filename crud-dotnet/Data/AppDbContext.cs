using crud_dotnet.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Produto
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Name).HasColumnName("name");
            entity.Property(p => p.Price).HasColumnName("price");
            entity.Property(p => p.Category).HasColumnName("category").HasConversion<string>();
        });

        // Orders
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.Property(o => o.Id).HasColumnName("id");
            entity.Property(o => o.CreatedAt).HasColumnName("created_at");
            entity.Property(o => o.TotalValue).HasColumnName("total_value");
            entity.Property(o => o.DiscountValue).HasColumnName("discount_value");
            entity.ToTable("orders");
            entity.Property(o => o.Status)
              .HasConversion<string>()
              .HasColumnName("status")
              .HasColumnType("status_enum");
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_item");
            entity.Property(oi => oi.Id).HasColumnName("id");
            entity.Property(oi => oi.OrderId).HasColumnName("order_id");
            entity.Property(oi => oi.ProductId).HasColumnName("product_id");
            entity.Property(oi => oi.Quantity).HasColumnName("quantity");
            entity.Property(oi => oi.UnitPrice).HasColumnName("unit_price");

            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(oi => oi.ProductId);
        });
    }
}
