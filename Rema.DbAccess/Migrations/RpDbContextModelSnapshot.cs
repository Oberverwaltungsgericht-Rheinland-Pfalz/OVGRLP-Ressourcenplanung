﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rema.DbAccess;

namespace Rema.DbAccess.Migrations
{
    [DbContext(typeof(RpDbContext))]
    partial class RpDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("AllocationGadget", b =>
                {
                    b.Property<long>("AllocationsId")
                        .HasColumnType("bigint");

                    b.Property<long>("GadgetsId")
                        .HasColumnType("bigint");

                    b.HasKey("AllocationsId", "GadgetsId");

                    b.HasIndex("GadgetsId");

                    b.ToTable("AllocationGadget");
                });

            modelBuilder.Entity("AllocationRessource", b =>
                {
                    b.Property<long>("AllocationsId")
                        .HasColumnType("bigint");

                    b.Property<long>("RessourcesId")
                        .HasColumnType("bigint");

                    b.HasKey("AllocationsId", "RessourcesId");

                    b.HasIndex("RessourcesId");

                    b.ToTable("AllocationRessource");
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.Allocation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<DateTime>("ApprovedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("ApprovedById")
                        .HasColumnType("bigint");

                    b.Property<string>("ContactPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("CreatedById")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("From")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAllDay")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<long?>("LastModifiedById")
                        .HasColumnType("bigint");

                    b.Property<string>("Notes")
                        .HasMaxLength(3000)
                        .HasColumnType("nvarchar(3000)");

                    b.Property<long?>("ReferencePersonId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Reminded")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ScheduleSeriesGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SerializedHints")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("To")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApprovedById");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LastModifiedById");

                    b.HasIndex("ReferencePersonId");

                    b.ToTable("Allocations");
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.Gadget", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<long?>("SuppliedById")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SuppliedById");

                    b.ToTable("Gadgets");
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.Ressource", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("FunctionDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SpecialsDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Ressources");
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.SupplierGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("GroupEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Title");

                    b.ToTable("SupplierGroups");
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("ActiveDirectoryID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Organisation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AllocationGadget", b =>
                {
                    b.HasOne("Rema.Infrastructure.Models.Allocation", null)
                        .WithMany()
                        .HasForeignKey("AllocationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Rema.Infrastructure.Models.Gadget", null)
                        .WithMany()
                        .HasForeignKey("GadgetsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AllocationRessource", b =>
                {
                    b.HasOne("Rema.Infrastructure.Models.Allocation", null)
                        .WithMany()
                        .HasForeignKey("AllocationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Rema.Infrastructure.Models.Ressource", null)
                        .WithMany()
                        .HasForeignKey("RessourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.Allocation", b =>
                {
                    b.HasOne("Rema.Infrastructure.Models.User", "ApprovedBy")
                        .WithMany()
                        .HasForeignKey("ApprovedById");

                    b.HasOne("Rema.Infrastructure.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Rema.Infrastructure.Models.User", "LastModifiedBy")
                        .WithMany()
                        .HasForeignKey("LastModifiedById");

                    b.HasOne("Rema.Infrastructure.Models.User", "ReferencePerson")
                        .WithMany()
                        .HasForeignKey("ReferencePersonId");

                    b.Navigation("ApprovedBy");

                    b.Navigation("CreatedBy");

                    b.Navigation("LastModifiedBy");

                    b.Navigation("ReferencePerson");
                });

            modelBuilder.Entity("Rema.Infrastructure.Models.Gadget", b =>
                {
                    b.HasOne("Rema.Infrastructure.Models.SupplierGroup", "SuppliedBy")
                        .WithMany()
                        .HasForeignKey("SuppliedById");

                    b.Navigation("SuppliedBy");
                });
#pragma warning restore 612, 618
        }
    }
}
