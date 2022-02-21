﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Scanner.Data;

namespace Scanner.Data.Migrations
{
    [DbContext(typeof(ScannerDB))]
    [Migration("20220220120659_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.13");

            modelBuilder.Entity("Scanner.Models.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("DocumentType")
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("IndexingDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Scanner.Models.DocumentMetadata", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DocumentId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("Metadata");
                });

            modelBuilder.Entity("Scanner.Models.FileData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Checked")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DocumentId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DocumentName")
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Indexed")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("FileDatas");
                });

            modelBuilder.Entity("Scanner.Models.ScannerDataTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("DocumentType")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DocumentType")
                        .IsUnique();

                    b.ToTable("DataTemplates");
                });

            modelBuilder.Entity("Scanner.Models.TemplateMetadata", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Required")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ScannerDataTemplateId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ScannerDataTemplateId");

                    b.ToTable("TemplateMetadata");
                });

            modelBuilder.Entity("Scanner.Models.DocumentMetadata", b =>
                {
                    b.HasOne("Scanner.Models.Document", "Document")
                        .WithMany("Metadata")
                        .HasForeignKey("DocumentId");

                    b.Navigation("Document");
                });

            modelBuilder.Entity("Scanner.Models.FileData", b =>
                {
                    b.HasOne("Scanner.Models.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId");

                    b.Navigation("Document");
                });

            modelBuilder.Entity("Scanner.Models.TemplateMetadata", b =>
                {
                    b.HasOne("Scanner.Models.ScannerDataTemplate", "ScannerDataTemplate")
                        .WithMany("TemplateMetadata")
                        .HasForeignKey("ScannerDataTemplateId");

                    b.Navigation("ScannerDataTemplate");
                });

            modelBuilder.Entity("Scanner.Models.Document", b =>
                {
                    b.Navigation("Metadata");
                });

            modelBuilder.Entity("Scanner.Models.ScannerDataTemplate", b =>
                {
                    b.Navigation("TemplateMetadata");
                });
#pragma warning restore 612, 618
        }
    }
}