using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dddify.Admin.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementsAndInboxItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sys_user_gender",
                table: "sys_user");

            migrationBuilder.DropIndex(
                name: "ix_sys_user_status",
                table: "sys_user");

            migrationBuilder.DropIndex(
                name: "ix_sys_role_is_default",
                table: "sys_role");

            migrationBuilder.DropIndex(
                name: "ix_sys_role_order",
                table: "sys_role");

            migrationBuilder.DropIndex(
                name: "ix_sys_lookup_item_lookup_id_order",
                table: "sys_lookup_item");

            migrationBuilder.DropIndex(
                name: "ix_sys_department_parent_id_order",
                table: "sys_department");

            migrationBuilder.CreateTable(
                name: "sys_announcement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_html = table.Column<string>(type: "text", nullable: false),
                    content_plain_text = table.Column<string>(type: "text", nullable: false),
                    audience_type = table.Column<int>(type: "integer", nullable: false),
                    audience_target_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    published_by = table.Column<Guid>(type: "uuid", nullable: true),
                    withdrawn_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    withdrawn_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_announcement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_inbox_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_inbox_item", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "sys_permission",
                columns: new[] { "id", "code", "created_at", "created_by", "deleted_at", "deleted_by", "is_deleted", "modified_at", "modified_by", "name", "order", "parent_id", "type" },
                values: new object[,]
                {
                    { new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), "system:announcement:index", null, null, null, null, false, null, null, "公告管理", 31, new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "Menu" },
                    { new Guid("019a9f3a-6615-7c6d-a0fb-350d77272465"), "system:announcement:create", null, null, null, null, false, null, null, "新增公告", 32, new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), "Button" },
                    { new Guid("019a9f3a-6615-7d0c-b85d-d9a9257c1ad1"), "system:announcement:update", null, null, null, null, false, null, null, "编辑公告", 33, new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), "Button" },
                    { new Guid("019a9f3a-6615-7db9-a376-bae30f3035f5"), "system:announcement:publish", null, null, null, null, false, null, null, "发布公告", 34, new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), "Button" },
                    { new Guid("019a9f3a-6615-7e58-ad93-7e450fa9f354"), "system:announcement:withdraw", null, null, null, null, false, null, null, "撤回公告", 35, new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), "Button" },
                    { new Guid("019a9f3a-6615-7f1f-98a6-a362654f26c8"), "system:announcement:delete", null, null, null, null, false, null, null, "删除公告", 36, new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), "Button" }
                });

            migrationBuilder.InsertData(
                table: "sys_role_permission",
                columns: new[] { "permission_id", "role_id", "permission_code" },
                values: new object[,]
                {
                    { new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:announcement:index" },
                    { new Guid("019a9f3a-6615-7c6d-a0fb-350d77272465"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:announcement:create" },
                    { new Guid("019a9f3a-6615-7d0c-b85d-d9a9257c1ad1"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:announcement:update" },
                    { new Guid("019a9f3a-6615-7db9-a376-bae30f3035f5"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:announcement:publish" },
                    { new Guid("019a9f3a-6615-7e58-ad93-7e450fa9f354"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:announcement:withdraw" },
                    { new Guid("019a9f3a-6615-7f1f-98a6-a362654f26c8"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:announcement:delete" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_is_deleted_created_at",
                table: "sys_user",
                columns: new[] { "is_deleted", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_is_deleted_gender_created_at",
                table: "sys_user",
                columns: new[] { "is_deleted", "gender", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_is_deleted_status_created_at",
                table: "sys_user",
                columns: new[] { "is_deleted", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_is_deleted_is_default_order",
                table: "sys_role",
                columns: new[] { "is_deleted", "is_default", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_is_deleted_order",
                table: "sys_role",
                columns: new[] { "is_deleted", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_permission_code",
                table: "sys_permission",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_permission_is_deleted_order",
                table: "sys_permission",
                columns: new[] { "is_deleted", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_permission_parent_id_name",
                table: "sys_permission",
                columns: new[] { "parent_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_item_lookup_id_is_deleted_is_enabled_order",
                table: "sys_lookup_item",
                columns: new[] { "lookup_id", "is_deleted", "is_enabled", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_item_lookup_id_is_deleted_order",
                table: "sys_lookup_item",
                columns: new[] { "lookup_id", "is_deleted", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_is_deleted_created_at",
                table: "sys_lookup",
                columns: new[] { "is_deleted", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_is_deleted_order",
                table: "sys_department",
                columns: new[] { "is_deleted", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_is_deleted_type_is_enabled_order",
                table: "sys_department",
                columns: new[] { "is_deleted", "type", "is_enabled", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_parent_id_is_deleted_order",
                table: "sys_department",
                columns: new[] { "parent_id", "is_deleted", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_announcement_is_deleted_created_at",
                table: "sys_announcement",
                columns: new[] { "is_deleted", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_announcement_is_deleted_status_created_at",
                table: "sys_announcement",
                columns: new[] { "is_deleted", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_inbox_item_source_type_source_id",
                table: "sys_inbox_item",
                columns: new[] { "source_type", "source_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_inbox_item_user_id_is_deleted_created_at",
                table: "sys_inbox_item",
                columns: new[] { "user_id", "is_deleted", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_inbox_item_user_id_is_deleted_is_read_created_at",
                table: "sys_inbox_item",
                columns: new[] { "user_id", "is_deleted", "is_read", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_announcement");

            migrationBuilder.DropTable(
                name: "sys_inbox_item");

            migrationBuilder.DropIndex(
                name: "ix_sys_user_is_deleted_created_at",
                table: "sys_user");

            migrationBuilder.DropIndex(
                name: "ix_sys_user_is_deleted_gender_created_at",
                table: "sys_user");

            migrationBuilder.DropIndex(
                name: "ix_sys_user_is_deleted_status_created_at",
                table: "sys_user");

            migrationBuilder.DropIndex(
                name: "ix_sys_role_is_deleted_is_default_order",
                table: "sys_role");

            migrationBuilder.DropIndex(
                name: "ix_sys_role_is_deleted_order",
                table: "sys_role");

            migrationBuilder.DropIndex(
                name: "ix_sys_permission_code",
                table: "sys_permission");

            migrationBuilder.DropIndex(
                name: "ix_sys_permission_is_deleted_order",
                table: "sys_permission");

            migrationBuilder.DropIndex(
                name: "ix_sys_permission_parent_id_name",
                table: "sys_permission");

            migrationBuilder.DropIndex(
                name: "ix_sys_lookup_item_lookup_id_is_deleted_is_enabled_order",
                table: "sys_lookup_item");

            migrationBuilder.DropIndex(
                name: "ix_sys_lookup_item_lookup_id_is_deleted_order",
                table: "sys_lookup_item");

            migrationBuilder.DropIndex(
                name: "ix_sys_lookup_is_deleted_created_at",
                table: "sys_lookup");

            migrationBuilder.DropIndex(
                name: "ix_sys_department_is_deleted_order",
                table: "sys_department");

            migrationBuilder.DropIndex(
                name: "ix_sys_department_is_deleted_type_is_enabled_order",
                table: "sys_department");

            migrationBuilder.DropIndex(
                name: "ix_sys_department_parent_id_is_deleted_order",
                table: "sys_department");

            migrationBuilder.DeleteData(
                table: "sys_permission",
                keyColumn: "id",
                keyValue: new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"));

            migrationBuilder.DeleteData(
                table: "sys_permission",
                keyColumn: "id",
                keyValue: new Guid("019a9f3a-6615-7c6d-a0fb-350d77272465"));

            migrationBuilder.DeleteData(
                table: "sys_permission",
                keyColumn: "id",
                keyValue: new Guid("019a9f3a-6615-7d0c-b85d-d9a9257c1ad1"));

            migrationBuilder.DeleteData(
                table: "sys_permission",
                keyColumn: "id",
                keyValue: new Guid("019a9f3a-6615-7db9-a376-bae30f3035f5"));

            migrationBuilder.DeleteData(
                table: "sys_permission",
                keyColumn: "id",
                keyValue: new Guid("019a9f3a-6615-7e58-ad93-7e450fa9f354"));

            migrationBuilder.DeleteData(
                table: "sys_permission",
                keyColumn: "id",
                keyValue: new Guid("019a9f3a-6615-7f1f-98a6-a362654f26c8"));

            migrationBuilder.DeleteData(
                table: "sys_role_permission",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("019a9f3a-6615-7c0f-bfe3-5da758f52b1f"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4") });

            migrationBuilder.DeleteData(
                table: "sys_role_permission",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("019a9f3a-6615-7c6d-a0fb-350d77272465"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4") });

            migrationBuilder.DeleteData(
                table: "sys_role_permission",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("019a9f3a-6615-7d0c-b85d-d9a9257c1ad1"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4") });

            migrationBuilder.DeleteData(
                table: "sys_role_permission",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("019a9f3a-6615-7db9-a376-bae30f3035f5"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4") });

            migrationBuilder.DeleteData(
                table: "sys_role_permission",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("019a9f3a-6615-7e58-ad93-7e450fa9f354"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4") });

            migrationBuilder.DeleteData(
                table: "sys_role_permission",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("019a9f3a-6615-7f1f-98a6-a362654f26c8"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4") });

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_gender",
                table: "sys_user",
                column: "gender");

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_status",
                table: "sys_user",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_is_default",
                table: "sys_role",
                column: "is_default");

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_order",
                table: "sys_role",
                column: "order");

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_item_lookup_id_order",
                table: "sys_lookup_item",
                columns: new[] { "lookup_id", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_parent_id_order",
                table: "sys_department",
                columns: new[] { "parent_id", "order" });
        }
    }
}
