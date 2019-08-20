using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DbRaumplanung.Models;

namespace DbRaumplanung.DataAccess
{
    public class RpDbContext: DbContext
    {
        public DbSet<Allocation> Allocations{ get; set; }
        public DbSet<AllocationPurpose> AllocationPurposes { get; set; }
        public DbSet<Gadget> Gadgets{ get; set; }
        public DbSet<Ressource> Ressources{ get; set; }
        public DbSet<SupplierGroup> SupplierGroups{ get; set; }
        public DbSet<User> Users { get; set; }

        public RpDbContext(DbContextOptions<RpDbContext> options) : base(options) {
           // options.UseSqlServer(connection, b => b.MigrationsAssembly("AspNetCoreVueStarter"));
        }
        public RpDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ressource>().HasIndex(e => e.Name);
        }
    }
}
