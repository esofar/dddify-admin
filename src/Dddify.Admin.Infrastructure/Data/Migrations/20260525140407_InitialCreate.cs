using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dddify.Admin.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_department",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    full_name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    path = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    leader_id = table.Column<Guid>(type: "uuid", nullable: false),
                    leader_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
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
                    table.PrimaryKey("pk_sys_department", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_lookup",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("pk_sys_lookup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_sys_permission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_preset = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    assigned_user_count = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
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
                    table.PrimaryKey("pk_sys_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    device_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token_hash = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: false),
                    previous_refresh_token_hash = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: true),
                    is_persistent = table.Column<bool>(type: "boolean", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_reason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nick_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_built_in = table.Column<bool>(type: "boolean", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_sys_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_lookup_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lookup_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    is_preset = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_sys_lookup_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_sys_lookup_item_sys_lookup_lookup_id",
                        column: x => x.lookup_id,
                        principalTable: "sys_lookup",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sys_role_permission",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_role_permission", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_sys_role_permission_sys_role_role_id",
                        column: x => x.role_id,
                        principalTable: "sys_role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sys_user_role",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_sys_user_role_sys_user_user_id",
                        column: x => x.user_id,
                        principalTable: "sys_user",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "sys_department",
                columns: new[] { "id", "code", "concurrency_stamp", "created_at", "created_by", "deleted_at", "deleted_by", "full_name", "is_deleted", "is_enabled", "level", "modified_at", "modified_by", "name", "order", "parent_id", "path", "type", "leader_id", "leader_name" },
                values: new object[,]
                {
                    { new Guid("08da692f-4718-401c-84c5-db3341edf972"), "Pk2d2L", null, null, null, null, null, "星瀚科技集团", false, true, 0, null, null, "星瀚科技集团", 1, null, "/Pk2d2L/", "headquarters", new Guid("3a05d6f8-42ef-02da-f267-94a48964c698"), "顾承远" },
                    { new Guid("08daf1f8-efff-4189-82f6-02184b401bbc"), "fWY0m0", null, null, null, null, null, "星瀚科技集团/产品中心", false, true, 1, null, null, "产品中心", 1, new Guid("08da692f-4718-401c-84c5-db3341edf972"), "/Pk2d2L/fWY0m0/", "business", new Guid("eb426312-7e53-4a0b-98df-2c1a4a4c432e"), "沈知夏" },
                    { new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"), "tt7y07", null, null, null, null, null, "星瀚科技集团/技术中心", false, true, 1, null, null, "技术中心", 2, new Guid("08da692f-4718-401c-84c5-db3341edf972"), "/Pk2d2L/tt7y07/", "business", new Guid("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), "陆景行" },
                    { new Guid("08daf20c-bde9-4d77-8a1e-6460f9a28d71"), "JZC2A1", null, null, null, null, null, "星瀚科技集团/技术中心/测试质量部", false, true, 2, null, null, "测试质量部", 3, new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"), "/Pk2d2L/tt7y07/JZC2A1/", "technology", new Guid("c55c35a1-84a2-419a-8db8-93b1c3b02bb9"), "林若宁" },
                    { new Guid("282d1497-7d7a-4029-86f0-6ac935010e4f"), "8iI3CK", null, null, null, null, null, "星瀚科技集团/技术中心/基础架构部", false, true, 2, null, null, "基础架构部", 2, new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"), "/Pk2d2L/tt7y07/8iI3CK/", "technology", new Guid("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), "陆景行" },
                    { new Guid("392a90cc-da28-4535-bf7b-cceb25938c4b"), "HhN4o9", null, null, null, null, null, "星瀚科技集团/职能中心", false, true, 1, null, null, "职能中心", 3, new Guid("08da692f-4718-401c-84c5-db3341edf972"), "/Pk2d2L/HhN4o9/", "business", new Guid("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), "韩叙白" },
                    { new Guid("3ad207b4-762c-42b3-acae-62f3665db5df"), "dJFLWU", null, null, null, null, null, "星瀚科技集团/职能中心/人力资源部", false, true, 2, null, null, "人力资源部", 2, new Guid("392a90cc-da28-4535-bf7b-cceb25938c4b"), "/Pk2d2L/HhN4o9/dJFLWU/", "human_resource", new Guid("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), "韩叙白" },
                    { new Guid("430aaead-e559-41b0-9079-cf3875f06004"), "TJj7qx", null, null, null, null, null, "星瀚科技集团/产品中心/AI 产品组", false, true, 2, null, null, "AI 产品组", 1, new Guid("08daf1f8-efff-4189-82f6-02184b401bbc"), "/Pk2d2L/fWY0m0/TJj7qx/", "product", new Guid("eb426312-7e53-4a0b-98df-2c1a4a4c432e"), "沈知夏" },
                    { new Guid("48c316ea-ed0c-4e4e-b2fb-83ab11a78dad"), "88H8OW", null, null, null, null, null, "星瀚科技集团/技术中心/平台研发部", false, true, 2, null, null, "平台研发部", 1, new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"), "/Pk2d2L/tt7y07/88H8OW/", "technology", new Guid("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), "陆景行" },
                    { new Guid("6c2b4d3f-755e-4b69-a16c-12906a9015e7"), "ybQKAw", null, null, null, null, null, "星瀚科技集团/产品中心/用户增长组", false, true, 2, null, null, "用户增长组", 2, new Guid("08daf1f8-efff-4189-82f6-02184b401bbc"), "/Pk2d2L/fWY0m0/ybQKAw/", "product", new Guid("c1e9475a-59d7-4a3c-b68d-8c144f403a51"), "唐雨桐" },
                    { new Guid("93a9e788-7357-4afe-9fbe-8477a74e904f"), "ctgmoD", null, null, null, null, null, "星瀚科技集团/职能中心/行政部", false, true, 2, null, null, "行政部", 3, new Guid("392a90cc-da28-4535-bf7b-cceb25938c4b"), "/Pk2d2L/HhN4o9/ctgmoD/", "administration", new Guid("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), "韩叙白" },
                    { new Guid("ff54b97f-d686-495e-9e1c-f0dc9249720e"), "Jd7S04", null, null, null, null, null, "星瀚科技集团/职能中心/财务部", false, true, 2, null, null, "财务部", 1, new Guid("392a90cc-da28-4535-bf7b-cceb25938c4b"), "/Pk2d2L/HhN4o9/Jd7S04/", "finance", new Guid("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), "韩叙白" }
                });

            migrationBuilder.InsertData(
                table: "sys_lookup",
                columns: new[] { "id", "code", "created_at", "created_by", "deleted_at", "deleted_by", "description", "is_deleted", "modified_at", "modified_by", "name" },
                values: new object[] { new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), "department_type", null, null, null, null, "用于定义部门分类，便于管理与区分。", false, null, null, "部门类型" });

            migrationBuilder.InsertData(
                table: "sys_permission",
                columns: new[] { "id", "code", "created_at", "created_by", "deleted_at", "deleted_by", "is_deleted", "modified_at", "modified_by", "name", "order", "parent_id", "type" },
                values: new object[,]
                {
                    { new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "system", null, null, null, null, false, null, null, "系统管理", 1, null, "Catalog" },
                    { new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "system:user:index", null, null, null, null, false, null, null, "用户管理", 1, new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "Menu" },
                    { new Guid("018f69e2-55a5-7c7b-a7a6-c2b2beabc408"), "system:role:index", null, null, null, null, false, null, null, "角色管理", 9, new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "Menu" },
                    { new Guid("018f69e2-55a6-7c7b-818b-93ea29100998"), "system:permission:index", null, null, null, null, false, null, null, "权限管理", 14, new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "Menu" },
                    { new Guid("018f69e2-55a7-7c7b-9c64-c9f62a84ec7b"), "system:department:index", null, null, null, null, false, null, null, "部门管理", 18, new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "Menu" },
                    { new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "system:lookup:index", null, null, null, null, false, null, null, "字典管理", 22, new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), "Menu" },
                    { new Guid("01979d24-ea82-708b-997a-1311dfba482b"), "system:user:create", null, null, null, null, false, null, null, "新增用户", 2, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-70cf-950c-32cc434d18f2"), "system:user:update", null, null, null, null, false, null, null, "编辑用户", 3, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-7286-b804-cea0cf28a01a"), "system:user:delete", null, null, null, null, false, null, null, "删除用户", 4, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-73e6-9e25-5004391febce"), "system:user:enable", null, null, null, null, false, null, null, "启用用户", 5, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-748d-bd21-c8faee1b6642"), "system:user:disable", null, null, null, null, false, null, null, "禁用用户", 6, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-7493-a171-cebf1e23c98c"), "system:user:reset-password", null, null, null, null, false, null, null, "重置密码", 7, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-749e-939a-c43c0a851959"), "system:user:assign-roles", null, null, null, null, false, null, null, "分配角色", 8, new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), "Button" },
                    { new Guid("01979d24-ea82-74c0-9a62-49ca0cbfe12b"), "system:role:create", null, null, null, null, false, null, null, "新增角色", 10, new Guid("018f69e2-55a5-7c7b-a7a6-c2b2beabc408"), "Button" },
                    { new Guid("01979d24-ea82-7536-8d50-c1b5bc023503"), "system:role:update", null, null, null, null, false, null, null, "编辑角色", 11, new Guid("018f69e2-55a5-7c7b-a7a6-c2b2beabc408"), "Button" },
                    { new Guid("01979d24-ea82-75a5-b394-e39d25d01626"), "system:role:delete", null, null, null, null, false, null, null, "删除角色", 12, new Guid("018f69e2-55a5-7c7b-a7a6-c2b2beabc408"), "Button" },
                    { new Guid("01979d24-ea82-75ee-b48c-205c452262bb"), "system:role:assign-permissions", null, null, null, null, false, null, null, "权限配置", 13, new Guid("018f69e2-55a5-7c7b-a7a6-c2b2beabc408"), "Button" },
                    { new Guid("01979d24-ea82-7687-b31a-46586630b624"), "system:permission:create", null, null, null, null, false, null, null, "新增权限", 15, new Guid("018f69e2-55a6-7c7b-818b-93ea29100998"), "Button" },
                    { new Guid("01979d24-ea82-77ea-abc3-a15b515e5b80"), "system:permission:update", null, null, null, null, false, null, null, "编辑权限", 16, new Guid("018f69e2-55a6-7c7b-818b-93ea29100998"), "Button" },
                    { new Guid("01979d24-ea82-780e-814c-e62cc6680b3a"), "system:permission:delete", null, null, null, null, false, null, null, "删除权限", 17, new Guid("018f69e2-55a6-7c7b-818b-93ea29100998"), "Button" },
                    { new Guid("01979d24-ea82-7858-b328-feb7c02db5c7"), "system:department:create", null, null, null, null, false, null, null, "新增部门", 19, new Guid("018f69e2-55a7-7c7b-9c64-c9f62a84ec7b"), "Button" },
                    { new Guid("01979d24-ea82-79ce-9036-7793446abb1b"), "system:department:update", null, null, null, null, false, null, null, "编辑部门", 20, new Guid("018f69e2-55a7-7c7b-9c64-c9f62a84ec7b"), "Button" },
                    { new Guid("01979d24-ea82-7ac3-aabb-23ba54940dc9"), "system:department:delete", null, null, null, null, false, null, null, "删除部门", 21, new Guid("018f69e2-55a7-7c7b-9c64-c9f62a84ec7b"), "Button" },
                    { new Guid("01979d24-ea82-7ae3-a345-b896e6392195"), "system:lookup:create", null, null, null, null, false, null, null, "新增字典", 23, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("01979d24-ea82-7b63-ad5e-9d8e917d3e5c"), "system:lookup:update", null, null, null, null, false, null, null, "编辑字典", 24, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("01979d24-ea82-7c14-bc58-18f42c8be279"), "system:lookup:delete", null, null, null, null, false, null, null, "删除字典", 25, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("01979d24-ea82-7c77-9c92-be1b4413c498"), "system:lookup:item:create", null, null, null, null, false, null, null, "新增字典项", 26, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("01979d24-ea82-7cf9-a242-88bd19882f78"), "system:lookup:item:update", null, null, null, null, false, null, null, "修改字典项", 27, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("01979d24-ea82-7d29-ab3e-aab3d1c996df"), "system:lookup:item:disable", null, null, null, null, false, null, null, "禁用字典项", 28, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("019a075c-8b2e-7f8d-a678-95c158f2fc6d"), "system:lookup:item:enable", null, null, null, null, false, null, null, "启用字典项", 29, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" },
                    { new Guid("019a48f8-d4c7-7049-bb57-15e38c00f979"), "system:lookup:item:delete", null, null, null, null, false, null, null, "删除字典项", 30, new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), "Button" }
                });

            migrationBuilder.InsertData(
                table: "sys_role",
                columns: new[] { "id", "assigned_user_count", "concurrency_stamp", "created_at", "created_by", "deleted_at", "deleted_by", "description", "is_default", "is_deleted", "is_preset", "modified_at", "modified_by", "name", "order" },
                values: new object[,]
                {
                    { new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), 1, null, null, null, null, null, "拥有系统最高权限，管理所有模块和用户。", false, false, true, null, null, "超级管理员", 1 },
                    { new Guid("018f0d8b-c695-7c32-866e-f7cf8ec9fcfc"), 0, null, null, null, null, null, "可管理用户、角色、权限、配置等，权限略低于超级管理员。", false, false, true, null, null, "管理员", 2 },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), 0, null, null, null, null, null, "系统基础用户，使用系统提供的基本功能。", true, false, true, null, null, "普通用户", 3 },
                    { new Guid("018f0d8b-c697-7c32-8675-7ec3e4eb6c9a"), 0, null, null, null, null, null, "只读权限，不能进行任何修改操作。", false, false, true, null, null, "访客", 4 }
                });

            migrationBuilder.InsertData(
                table: "sys_user",
                columns: new[] { "id", "avatar", "birth_date", "concurrency_stamp", "created_at", "created_by", "deleted_at", "deleted_by", "email", "gender", "is_built_in", "is_deleted", "last_login_at", "modified_at", "modified_by", "name", "nick_name", "password_hash", "phone_number", "status", "department_id", "department_name" },
                values: new object[,]
                {
                    { new Guid("3a05d6f8-42ef-02da-f267-94a48964c698"), "https://api.dicebear.com/7.x/avataaars/svg?seed=chengyuan-gu", null, null, null, null, null, null, "chengyuan.gu@xinghan.tech", "Male", true, false, null, null, null, "顾承远", "Chengyuan Gu", "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm", "13800138001", "Enabled", new Guid("08da692f-4718-401c-84c5-db3341edf972"), "星瀚科技集团" },
                    { new Guid("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), "https://api.dicebear.com/7.x/avataaars/svg?seed=xubai-han", null, null, null, null, null, null, "xubai.han@xinghan.tech", "Male", false, false, null, null, null, "韩叙白", "Xubai Han", "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm", "13800138006", "Enabled", new Guid("392a90cc-da28-4535-bf7b-cceb25938c4b"), "职能中心" },
                    { new Guid("c1e9475a-59d7-4a3c-b68d-8c144f403a51"), "https://api.dicebear.com/7.x/avataaars/svg?seed=yutong-tang", null, null, null, null, null, null, "yutong.tang@xinghan.tech", "Female", false, false, null, null, null, "唐雨桐", "Yutong Tang", "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm", "13800138002", "Enabled", new Guid("6c2b4d3f-755e-4b69-a16c-12906a9015e7"), "用户增长组" },
                    { new Guid("c55c35a1-84a2-419a-8db8-93b1c3b02bb9"), "https://api.dicebear.com/7.x/avataaars/svg?seed=ruoning-lin", null, null, null, null, null, null, "ruoning.lin@xinghan.tech", "Female", false, false, null, null, null, "林若宁", "Ruoning Lin", "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm", "13800138005", "Enabled", new Guid("08daf20c-bde9-4d77-8a1e-6460f9a28d71"), "测试质量部" },
                    { new Guid("eb426312-7e53-4a0b-98df-2c1a4a4c432e"), "https://api.dicebear.com/7.x/avataaars/svg?seed=zhixia-shen", null, null, null, null, null, null, "zhixia.shen@xinghan.tech", "Female", false, false, null, null, null, "沈知夏", "Zhixia Shen", "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm", "13800138003", "Enabled", new Guid("08daf1f8-efff-4189-82f6-02184b401bbc"), "产品中心" },
                    { new Guid("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), "https://api.dicebear.com/7.x/avataaars/svg?seed=jingxing-lu", null, null, null, null, null, null, "jingxing.lu@xinghan.tech", "Male", false, false, null, null, null, "陆景行", "Jingxing Lu", "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm", "13800138004", "Enabled", new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"), "技术中心" }
                });

            migrationBuilder.InsertData(
                table: "sys_lookup_item",
                columns: new[] { "id", "color", "created_at", "created_by", "deleted_at", "deleted_by", "is_deleted", "is_enabled", "is_preset", "label", "lookup_id", "modified_at", "modified_by", "order", "value" },
                values: new object[,]
                {
                    { new Guid("019e3b16-b41b-7780-bc9d-e46e02359074"), null, null, null, null, null, false, true, true, "总部", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 1, "headquarters" },
                    { new Guid("019e3b16-b41b-7905-9176-d3bbcce11e83"), null, null, null, null, null, false, true, true, "业务部门", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 3, "business" },
                    { new Guid("019e3b16-b41c-75c8-bbd2-64834915b320"), null, null, null, null, null, false, true, true, "技术部门", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 5, "technology" },
                    { new Guid("019e3b16-b41c-77b0-8471-5b98ea7cc354"), null, null, null, null, null, false, true, true, "产品部门", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 4, "product" },
                    { new Guid("019e3b16-b41f-759e-aed3-e723ce06a88d"), null, null, null, null, null, false, true, true, "财务部门", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 6, "finance" },
                    { new Guid("019e3b16-b420-7300-9645-db31c3391eef"), null, null, null, null, null, false, true, true, "人力资源部", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 7, "human_resource" },
                    { new Guid("019e3b16-b420-76f4-8e33-62fae4848a01"), null, null, null, null, null, false, true, true, "行政部门", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 8, "administration" },
                    { new Guid("019e3b16-b423-79c1-a37c-7524ae11a8d7"), null, null, null, null, null, false, true, true, "分公司", new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"), null, null, 2, "branch" }
                });

            migrationBuilder.InsertData(
                table: "sys_role_permission",
                columns: new[] { "permission_id", "role_id", "permission_code" },
                values: new object[,]
                {
                    { new Guid("018f69e2-55a3-7c7b-8c52-4d1e31f69b25"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system" },
                    { new Guid("018f69e2-55a4-7c7b-a56d-b16c37e51952"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:index" },
                    { new Guid("018f69e2-55a5-7c7b-a7a6-c2b2beabc408"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:role:index" },
                    { new Guid("018f69e2-55a6-7c7b-818b-93ea29100998"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:permission:index" },
                    { new Guid("018f69e2-55a7-7c7b-9c64-c9f62a84ec7b"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:department:index" },
                    { new Guid("018f69e2-55a8-7c7b-80b2-55d4a1216a89"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:index" },
                    { new Guid("01979d24-ea82-708b-997a-1311dfba482b"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:create" },
                    { new Guid("01979d24-ea82-70cf-950c-32cc434d18f2"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:update" },
                    { new Guid("01979d24-ea82-7286-b804-cea0cf28a01a"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:delete" },
                    { new Guid("01979d24-ea82-73e6-9e25-5004391febce"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:enable" },
                    { new Guid("01979d24-ea82-748d-bd21-c8faee1b6642"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:disable" },
                    { new Guid("01979d24-ea82-7493-a171-cebf1e23c98c"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:reset-password" },
                    { new Guid("01979d24-ea82-749e-939a-c43c0a851959"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:user:assign-roles" },
                    { new Guid("01979d24-ea82-74c0-9a62-49ca0cbfe12b"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:role:create" },
                    { new Guid("01979d24-ea82-7536-8d50-c1b5bc023503"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:role:update" },
                    { new Guid("01979d24-ea82-75a5-b394-e39d25d01626"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:role:delete" },
                    { new Guid("01979d24-ea82-75ee-b48c-205c452262bb"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:role:assign-permissions" },
                    { new Guid("01979d24-ea82-7687-b31a-46586630b624"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:permission:create" },
                    { new Guid("01979d24-ea82-77ea-abc3-a15b515e5b80"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:permission:update" },
                    { new Guid("01979d24-ea82-780e-814c-e62cc6680b3a"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:permission:delete" },
                    { new Guid("01979d24-ea82-7858-b328-feb7c02db5c7"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:department:create" },
                    { new Guid("01979d24-ea82-79ce-9036-7793446abb1b"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:department:update" },
                    { new Guid("01979d24-ea82-7ac3-aabb-23ba54940dc9"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:department:delete" },
                    { new Guid("01979d24-ea82-7ae3-a345-b896e6392195"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:create" },
                    { new Guid("01979d24-ea82-7b63-ad5e-9d8e917d3e5c"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:update" },
                    { new Guid("01979d24-ea82-7c14-bc58-18f42c8be279"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:delete" },
                    { new Guid("01979d24-ea82-7c77-9c92-be1b4413c498"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:item:create" },
                    { new Guid("01979d24-ea82-7cf9-a242-88bd19882f78"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:item:update" },
                    { new Guid("01979d24-ea82-7d29-ab3e-aab3d1c996df"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:item:disable" },
                    { new Guid("019a075c-8b2e-7f8d-a678-95c158f2fc6d"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:item:enable" },
                    { new Guid("019a48f8-d4c7-7049-bb57-15e38c00f979"), new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), "system:lookup:item:delete" }
                });

            migrationBuilder.InsertData(
                table: "sys_user_role",
                columns: new[] { "role_id", "user_id", "is_current", "role_name" },
                values: new object[,]
                {
                    { new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"), new Guid("3a05d6f8-42ef-02da-f267-94a48964c698"), true, "系统管理员" },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), new Guid("3a05d6f8-42ef-02da-f267-94a48964c698"), false, "普通用户" },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), new Guid("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), true, "普通用户" },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), new Guid("c1e9475a-59d7-4a3c-b68d-8c144f403a51"), true, "普通用户" },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), new Guid("c55c35a1-84a2-419a-8db8-93b1c3b02bb9"), true, "普通用户" },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), new Guid("eb426312-7e53-4a0b-98df-2c1a4a4c432e"), true, "普通用户" },
                    { new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"), new Guid("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), true, "普通用户" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_code",
                table: "sys_department",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_level",
                table: "sys_department",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_parent_id_name",
                table: "sys_department",
                columns: new[] { "parent_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_parent_id_order",
                table: "sys_department",
                columns: new[] { "parent_id", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_department_path",
                table: "sys_department",
                column: "path");

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_code",
                table: "sys_lookup",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_name",
                table: "sys_lookup",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_item_lookup_id_order",
                table: "sys_lookup_item",
                columns: new[] { "lookup_id", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_lookup_item_lookup_id_value",
                table: "sys_lookup_item",
                columns: new[] { "lookup_id", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_is_default",
                table: "sys_role",
                column: "is_default");

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_name",
                table: "sys_role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_order",
                table: "sys_role",
                column: "order");

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_permission_permission_code",
                table: "sys_role_permission",
                column: "permission_code");

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_permission_permission_id",
                table: "sys_role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_sys_session_expires_at",
                table: "sys_session",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_sys_session_previous_refresh_token_hash",
                table: "sys_session",
                column: "previous_refresh_token_hash");

            migrationBuilder.CreateIndex(
                name: "ix_sys_session_refresh_token_hash",
                table: "sys_session",
                column: "refresh_token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_session_revoked_at",
                table: "sys_session",
                column: "revoked_at");

            migrationBuilder.CreateIndex(
                name: "ix_sys_session_user_id",
                table: "sys_session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sys_session_user_id_device_id",
                table: "sys_session",
                columns: new[] { "user_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_department_id",
                table: "sys_user",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_email",
                table: "sys_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_gender",
                table: "sys_user",
                column: "gender");

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_phone_number",
                table: "sys_user",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_status",
                table: "sys_user",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_role_role_id",
                table: "sys_user_role",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_department");

            migrationBuilder.DropTable(
                name: "sys_lookup_item");

            migrationBuilder.DropTable(
                name: "sys_permission");

            migrationBuilder.DropTable(
                name: "sys_role_permission");

            migrationBuilder.DropTable(
                name: "sys_session");

            migrationBuilder.DropTable(
                name: "sys_user_role");

            migrationBuilder.DropTable(
                name: "sys_lookup");

            migrationBuilder.DropTable(
                name: "sys_role");

            migrationBuilder.DropTable(
                name: "sys_user");
        }
    }
}
