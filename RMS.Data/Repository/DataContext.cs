
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RMS.Data.Entities;

namespace RMS.Data.Repository;

public class DataContext : DbContext
{
    public DbSet<Menu> Menus { get; set; }
 
    public DbSet<User> Users { get; set; }

    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<AllergenConsent> AllergenConsents { get; set; }
    public DbSet<Review> Reviews { get; set; }

      
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder                              
            .UseSqlite("Filename=RMS.db")
            .LogTo(Console.WriteLine, LogLevel.Information)
            ;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>()
            .HasMany(mi => mi.Ingredients)
            .WithMany();

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public void Initialise() 
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
}
