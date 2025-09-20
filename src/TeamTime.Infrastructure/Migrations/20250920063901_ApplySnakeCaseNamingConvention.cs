using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplySnakeCaseNamingConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_assignments_projects_project_id",
                table: "project_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_project_assignments_users_assigned_by_id",
                table: "project_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_project_assignments_users_user_id",
                table: "project_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_areas_area_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_users_created_by",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_projects_project_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_users_created_by",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_time_entries_projects_project_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_time_entries_tasks_task_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_time_entries_users_approved_by_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_time_entries_users_user_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_users_areas_area_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_time_entries",
                table: "time_entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tasks",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_projects",
                table: "projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_project_assignments",
                table: "project_assignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_areas",
                table: "areas");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "ix_users_email");

            migrationBuilder.RenameIndex(
                name: "IX_users_area_id",
                table: "users",
                newName: "ix_users_area_id");

            migrationBuilder.RenameColumn(
                name: "Hours",
                table: "time_entries",
                newName: "hours");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "time_entries",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "time_entries",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "time_entries",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_time_entries_user_date",
                table: "time_entries",
                newName: "ix_time_entries_user_date");

            migrationBuilder.RenameIndex(
                name: "IX_time_entries_task_id",
                table: "time_entries",
                newName: "ix_time_entries_task_id");

            migrationBuilder.RenameIndex(
                name: "IX_time_entries_project_date",
                table: "time_entries",
                newName: "ix_time_entries_project_date");

            migrationBuilder.RenameIndex(
                name: "IX_time_entries_approved_by_id",
                table: "time_entries",
                newName: "ix_time_entries_approved_by_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tasks",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "tasks",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tasks",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "tasks",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tasks",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_project_id",
                table: "tasks",
                newName: "ix_tasks_project_id");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_created_by",
                table: "tasks",
                newName: "ix_tasks_created_by");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "projects",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "projects",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "projects",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "projects",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "projects",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_projects_created_by",
                table: "projects",
                newName: "ix_projects_created_by");

            migrationBuilder.RenameIndex(
                name: "IX_projects_area_id",
                table: "projects",
                newName: "ix_projects_area_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "project_assignments",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_project_assignments_user_id",
                table: "project_assignments",
                newName: "ix_project_assignments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_project_assignments_project_id_user_id_is_active",
                table: "project_assignments",
                newName: "ix_project_assignments_project_id_user_id_is_active");

            migrationBuilder.RenameIndex(
                name: "IX_project_assignments_assigned_by_id",
                table: "project_assignments",
                newName: "ix_project_assignments_assigned_by_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "areas",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "areas",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "areas",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "areas",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_areas_Name",
                table: "areas",
                newName: "ix_areas_name");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_time_entries",
                table: "time_entries",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tasks",
                table: "tasks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_projects",
                table: "projects",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_project_assignments",
                table: "project_assignments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_areas",
                table: "areas",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_project_assignments_projects_project_id",
                table: "project_assignments",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_project_assignments_users_assigned_by_id",
                table: "project_assignments",
                column: "assigned_by_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_project_assignments_users_user_id",
                table: "project_assignments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_areas_area_id",
                table: "projects",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_users_created_by",
                table: "projects",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_projects_project_id",
                table: "tasks",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_users_created_by",
                table: "tasks",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_projects_project_id",
                table: "time_entries",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_tasks_task_id",
                table: "time_entries",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_users_approved_by_id",
                table: "time_entries",
                column: "approved_by_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_users_user_id",
                table: "time_entries",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_areas_area_id",
                table: "users",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_assignments_projects_project_id",
                table: "project_assignments");

            migrationBuilder.DropForeignKey(
                name: "fk_project_assignments_users_assigned_by_id",
                table: "project_assignments");

            migrationBuilder.DropForeignKey(
                name: "fk_project_assignments_users_user_id",
                table: "project_assignments");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_areas_area_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_users_created_by",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_projects_project_id",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_users_created_by",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_projects_project_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_tasks_task_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_users_approved_by_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_users_user_id",
                table: "time_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_users_areas_area_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_time_entries",
                table: "time_entries");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tasks",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_projects",
                table: "projects");

            migrationBuilder.DropPrimaryKey(
                name: "pk_project_assignments",
                table: "project_assignments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_areas",
                table: "areas");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "ix_users_area_id",
                table: "users",
                newName: "IX_users_area_id");

            migrationBuilder.RenameColumn(
                name: "hours",
                table: "time_entries",
                newName: "Hours");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "time_entries",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "time_entries",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "time_entries",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_time_entries_user_date",
                table: "time_entries",
                newName: "IX_time_entries_user_date");

            migrationBuilder.RenameIndex(
                name: "ix_time_entries_task_id",
                table: "time_entries",
                newName: "IX_time_entries_task_id");

            migrationBuilder.RenameIndex(
                name: "ix_time_entries_project_date",
                table: "time_entries",
                newName: "IX_time_entries_project_date");

            migrationBuilder.RenameIndex(
                name: "ix_time_entries_approved_by_id",
                table: "time_entries",
                newName: "IX_time_entries_approved_by_id");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "tasks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "tasks",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "tasks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "tasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "tasks",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_project_id",
                table: "tasks",
                newName: "IX_tasks_project_id");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_created_by",
                table: "tasks",
                newName: "IX_tasks_created_by");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "projects",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "projects",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "projects",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_projects_created_by",
                table: "projects",
                newName: "IX_projects_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_projects_area_id",
                table: "projects",
                newName: "IX_projects_area_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "project_assignments",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_project_assignments_user_id",
                table: "project_assignments",
                newName: "IX_project_assignments_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_project_assignments_project_id_user_id_is_active",
                table: "project_assignments",
                newName: "IX_project_assignments_project_id_user_id_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_project_assignments_assigned_by_id",
                table: "project_assignments",
                newName: "IX_project_assignments_assigned_by_id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "areas",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "areas",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "areas",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "areas",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_areas_name",
                table: "areas",
                newName: "IX_areas_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_time_entries",
                table: "time_entries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tasks",
                table: "tasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_projects",
                table: "projects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_project_assignments",
                table: "project_assignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_areas",
                table: "areas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_project_assignments_projects_project_id",
                table: "project_assignments",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_assignments_users_assigned_by_id",
                table: "project_assignments",
                column: "assigned_by_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_project_assignments_users_user_id",
                table: "project_assignments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_areas_area_id",
                table: "projects",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_users_created_by",
                table: "projects",
                column: "created_by",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_projects_project_id",
                table: "tasks",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_users_created_by",
                table: "tasks",
                column: "created_by",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_time_entries_projects_project_id",
                table: "time_entries",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_time_entries_tasks_task_id",
                table: "time_entries",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_time_entries_users_approved_by_id",
                table: "time_entries",
                column: "approved_by_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_time_entries_users_user_id",
                table: "time_entries",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_areas_area_id",
                table: "users",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
