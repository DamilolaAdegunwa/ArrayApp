using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArrayApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class wwip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "TodoItems");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "TodoLists",
                newName: "LastModifierUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "TodoLists",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "TodoItems",
                newName: "LastModifierUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "TodoItems",
                newName: "CreatorUserId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "TodoLists",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "DeleterUserId",
                table: "TodoLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletionTime",
                table: "TodoLists",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TodoLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModificationTime",
                table: "TodoLists",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "TodoItems",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "DeleterUserId",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletionTime",
                table: "TodoItems",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TodoItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModificationTime",
                table: "TodoItems",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "TodoItems");

            migrationBuilder.RenameColumn(
                name: "LastModifierUserId",
                table: "TodoLists",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "TodoLists",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifierUserId",
                table: "TodoItems",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "TodoItems",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "TodoLists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "TodoLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "TodoItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "TodoItems",
                type: "datetime2",
                nullable: true);
        }
    }
}
