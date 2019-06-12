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
        public DbSet<SupplierGroup> supplierGroups{ get; set; }
        public DbSet<User> Users { get; set; }

        public RpDbContext(DbContextOptions<RpDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }
    }
}
