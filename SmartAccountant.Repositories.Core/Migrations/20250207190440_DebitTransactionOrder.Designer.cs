﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartAccountant.Repositories.Core.DataContexts;

#nullable disable

namespace SmartAccountant.Repositories.Core.Migrations
{
    [DbContext(typeof(CoreDbContext))]
    [Migration("20250207190440_DebitTransactionOrder")]
    partial class DebitTransactionOrder
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Bank")
                        .HasColumnType("smallint");

                    b.Property<string>("FriendlyName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("HolderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("HolderId");

                    b.ToTable("Accounts");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.Statement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("PeriodEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("PeriodStart")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Statements");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<short>("AmountCurrency")
                        .HasColumnType("smallint");

                    b.Property<string>("Note")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ReferenceNumber")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("StatementId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("StatementId");

                    b.ToTable("Transactions");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.SavingAccount", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Account");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.ToTable("SavingAccounts");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.DebitTransaction", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Transaction");

                    b.Property<short>("Order")
                        .HasColumnType("smallint");

                    b.Property<decimal>("RemainingAmount")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<short>("RemainingAmountCurrency")
                        .HasColumnType("smallint");

                    b.ToTable("DebitTransactions");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.Statement", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.Transaction", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Statement", "Statement")
                        .WithMany("Transactions")
                        .HasForeignKey("StatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Statement");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.SavingAccount", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Account", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.SavingAccount", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.DebitTransaction", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Transaction", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.DebitTransaction", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.Statement", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
