﻿// <auto-generated />
using System;
using Labb3Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Labb3Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240430093649_3tabeller")]
    partial class _3tabeller
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Labb3Api.Models.Hobby", b =>
                {
                    b.Property<int>("HobbyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HobbyId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PersonId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HobbyId");

                    b.HasIndex("PersonId");

                    b.ToTable("Hobbys");
                });

            modelBuilder.Entity("Labb3Api.Models.Link", b =>
                {
                    b.Property<int>("LinkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LinkId"));

                    b.Property<int?>("HobbyId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LinkId");

                    b.HasIndex("HobbyId");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("Labb3Api.Models.Person", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PersonId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phonenumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PersonId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Labb3Api.Models.Hobby", b =>
                {
                    b.HasOne("Labb3Api.Models.Person", null)
                        .WithMany("Hobbies")
                        .HasForeignKey("PersonId");
                });

            modelBuilder.Entity("Labb3Api.Models.Link", b =>
                {
                    b.HasOne("Labb3Api.Models.Hobby", null)
                        .WithMany("Links")
                        .HasForeignKey("HobbyId");
                });

            modelBuilder.Entity("Labb3Api.Models.Hobby", b =>
                {
                    b.Navigation("Links");
                });

            modelBuilder.Entity("Labb3Api.Models.Person", b =>
                {
                    b.Navigation("Hobbies");
                });
#pragma warning restore 612, 618
        }
    }
}
