﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NorthwindApp.FrontEnd.Mvc.Identity;

namespace NorthwindApp.FrontEnd.Mvc.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    [Migration("20220407202503_CreateIdentitySchema")]
    partial class CreateIdentitySchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NorthwindApp.FrontEnd.Mvc.Identity.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.ToTable("roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Employee"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Customer"
                        });
                });

            modelBuilder.Entity("NorthwindApp.FrontEnd.Mvc.Identity.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("email");

                    b.Property<int>("NorthwindId")
                        .HasColumnType("int")
                        .HasColumnName("northwind_id");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("password");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_users_role_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("NorthwindApp.FrontEnd.Mvc.Identity.Models.User", b =>
                {
                    b.HasOne("NorthwindApp.FrontEnd.Mvc.Identity.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_users_roles_role_id");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("NorthwindApp.FrontEnd.Mvc.Identity.Models.Role", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
