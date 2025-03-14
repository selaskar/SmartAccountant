﻿// <auto-generated />

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAccountant.Repositories.Core.Migrations
{
    /// <inheritdoc />
    internal partial class AddProvisionState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ProvisionState",
                table: "CreditCardTransactions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvisionState",
                table: "CreditCardTransactions");
        }
    }
}
