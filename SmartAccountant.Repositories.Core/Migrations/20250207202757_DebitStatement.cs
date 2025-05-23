﻿// <auto-generated/>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAccountant.Repositories.Core.Migrations
{
    /// <inheritdoc />
    internal partial class DebitStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Currency",
                table: "SavingAccounts",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "DebitStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Currency = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitStatements_Statements_Id",
                        column: x => x.Id,
                        principalTable: "Statements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebitStatements");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "SavingAccounts");
        }
    }
}
