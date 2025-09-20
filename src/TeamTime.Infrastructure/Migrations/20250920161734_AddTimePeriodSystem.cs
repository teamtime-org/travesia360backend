using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimePeriodSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Removed DropForeignKey operations - these constraints don't exist yet
            // migrationBuilder.DropForeignKey(
            //     name: "fk_time_entries_user_approved_by_id",
            //     table: "time_entries");

            // migrationBuilder.DropForeignKey(
            //     name: "fk_time_entries_user_user_id",
            //     table: "time_entries");

            migrationBuilder.AddColumn<Guid>(
                name: "time_period_id",
                table: "time_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Removed shadow state user_id columns - we'll fix user relationships later
            // migrationBuilder.AddColumn<Guid>(
            //     name: "user_id1",
            //     table: "time_entries",
            //     type: "uuid",
            //     nullable: true);

            // migrationBuilder.AddColumn<Guid>(
            //     name: "user_id2",
            //     table: "time_entries",
            //     type: "uuid",
            //     nullable: false,
            //     defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "time_periods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    reference_hours = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_current = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_time_periods", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_time_entries_time_period_date",
                table: "time_entries",
                columns: new[] { "time_period_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_time_entries_time_period_id",
                table: "time_entries",
                column: "time_period_id");

            // Removed indexes for shadow state user_id columns
            // migrationBuilder.CreateIndex(
            //     name: "ix_time_entries_user_id1",
            //     table: "time_entries",
            //     column: "user_id1");

            // migrationBuilder.CreateIndex(
            //     name: "ix_time_entries_user_id2",
            //     table: "time_entries",
            //     column: "user_id2");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_current_type",
                table: "time_periods",
                columns: new[] { "is_current", "type" },
                filter: "is_current = true");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_end_date",
                table: "time_periods",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_is_active",
                table: "time_periods",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_is_current",
                table: "time_periods",
                column: "is_current");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_start_date",
                table: "time_periods",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_type",
                table: "time_periods",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_time_periods_type_dates",
                table: "time_periods",
                columns: new[] { "type", "start_date", "end_date" });

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_time_periods_time_period_id",
                table: "time_entries",
                column: "time_period_id",
                principalTable: "time_periods",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // Removed foreign keys for shadow state user_id columns
            // migrationBuilder.AddForeignKey(
            //     name: "fk_time_entries_user_user_id1",
            //     table: "time_entries",
            //     column: "user_id1",
            //     principalTable: "user",
            //     principalColumn: "id");

            // migrationBuilder.AddForeignKey(
            //     name: "fk_time_entries_user_user_id2",
            //     table: "time_entries",
            //     column: "user_id2",
            //     principalTable: "user",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_time_periods_time_period_id",
                table: "time_entries");

            // Removed drop foreign key operations for shadow state columns
            // migrationBuilder.DropForeignKey(
            //     name: "fk_time_entries_user_user_id1",
            //     table: "time_entries");

            // migrationBuilder.DropForeignKey(
            //     name: "fk_time_entries_user_user_id2",
            //     table: "time_entries");

            migrationBuilder.DropTable(
                name: "time_periods");

            migrationBuilder.DropIndex(
                name: "ix_time_entries_time_period_date",
                table: "time_entries");

            migrationBuilder.DropIndex(
                name: "ix_time_entries_time_period_id",
                table: "time_entries");

            // Removed drop index operations for shadow state columns
            // migrationBuilder.DropIndex(
            //     name: "ix_time_entries_user_id1",
            //     table: "time_entries");

            // migrationBuilder.DropIndex(
            //     name: "ix_time_entries_user_id2",
            //     table: "time_entries");

            migrationBuilder.DropColumn(
                name: "time_period_id",
                table: "time_entries");

            // Removed drop column operations for shadow state columns
            // migrationBuilder.DropColumn(
            //     name: "user_id1",
            //     table: "time_entries");

            // migrationBuilder.DropColumn(
            //     name: "user_id2",
            //     table: "time_entries");

            // Removed AddForeignKey operations - these don't exist in current schema
            // migrationBuilder.AddForeignKey(
            //     name: "fk_time_entries_user_approved_by_id",
            //     table: "time_entries",
            //     column: "approved_by_id",
            //     principalTable: "user",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.SetNull);

            // migrationBuilder.AddForeignKey(
            //     name: "fk_time_entries_user_user_id",
            //     table: "time_entries",
            //     column: "user_id",
            //     principalTable: "user",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);
        }
    }
}
