using Microsoft.EntityFrameworkCore;
using Rema.Infrastructure.Models;

namespace Rema.DbAccess
{
  public class RpDbContext : DbContext
  {
    public DbSet<Allocation> Allocations { get; set; }
    public DbSet<Gadget> Gadgets { get; set; }
    public DbSet<Ressource> Ressources { get; set; }
    public DbSet<SupplierGroup> SupplierGroups { get; set; }
    public DbSet<User> Users { get; set; }

    public RpDbContext(DbContextOptions<RpDbContext> options) : base(options)
    {
      // options.UseSqlServer(connection, b => b.MigrationsAssembly("AspNetCoreVueStarter"));
    }

    public RpDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Ressource>().HasIndex(e => e.Name);
      modelBuilder.Entity<SupplierGroup>().HasIndex(e => e.Title);
      modelBuilder.Entity<User>().HasIndex(e => e.Email);
      modelBuilder.Entity<Allocation>().Property(Allocation.SerializedHintsExpression);
    }
  }
}
