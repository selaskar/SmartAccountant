﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartAccountant.Repositories.Core.DataContexts;

#nullable disable

namespace SmartAccountant.Repositories.Core.Migrations
{
    [DbContext(typeof(CoreDbContext))]
    partial class CoreDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
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

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCardLimit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ValidSince")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("ValidUntil")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("CreditCardLimits");
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

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.StatementDocument", b =>
                {
                    b.Property<Guid>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("StatementId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DocumentId");

                    b.HasIndex("StatementId");

                    b.ToTable("StatementDocuments");
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

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("PersonalNote")
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

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCard", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Account");

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.ToTable("CreditCards");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.SavingAccount", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Account");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<short>("Currency")
                        .HasColumnType("smallint");

                    b.ToTable("SavingAccounts");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCardStatement", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Statement");

                    b.Property<DateTimeOffset>("DueDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<decimal?>("MinimumDueAmount")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<decimal?>("RolloverAmount")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<decimal>("TotalDueAmount")
                        .HasColumnType("decimal(19, 4)");

                    b.Property<decimal?>("TotalFees")
                        .HasColumnType("decimal(19, 4)");

                    b.ToTable("CreditCardStatements");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.DebitStatement", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Statement");

                    b.Property<short>("Currency")
                        .HasColumnType("smallint");

                    b.ToTable("DebitStatements");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCardTransaction", b =>
                {
                    b.HasBaseType("SmartAccountant.Repositories.Core.Entities.Transaction");

                    b.ToTable("CreditCardTransactions");
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

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCardLimit", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.CreditCard", "Card")
                        .WithMany("Limits")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
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

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.StatementDocument", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Statement", "Statement")
                        .WithMany("Documents")
                        .HasForeignKey("StatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Statement");
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

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCard", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Account", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.CreditCard", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.SavingAccount", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Account", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.SavingAccount", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCardStatement", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Statement", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.CreditCardStatement", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.DebitStatement", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Statement", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.DebitStatement", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCardTransaction", b =>
                {
                    b.HasOne("SmartAccountant.Repositories.Core.Entities.Transaction", null)
                        .WithOne()
                        .HasForeignKey("SmartAccountant.Repositories.Core.Entities.CreditCardTransaction", "Id")
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
                    b.Navigation("Documents");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("SmartAccountant.Repositories.Core.Entities.CreditCard", b =>
                {
                    b.Navigation("Limits");
                });
#pragma warning restore 612, 618
        }
    }
}
