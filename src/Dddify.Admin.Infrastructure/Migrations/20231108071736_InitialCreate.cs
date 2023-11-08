using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dddify.Admin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "字典ID", collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, comment: "描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, comment: "并发标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionary", x => x.Id);
                },
                comment: "字典表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "机构ID", collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "上级ID", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "类型（字典编码：organization_type）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false, comment: "显示顺序"),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否启用"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, comment: "并发标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                },
                comment: "机构表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色ID", collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, comment: "描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, comment: "并发标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                },
                comment: "角色表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DictionaryItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "字典选项ID", collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false, comment: "显示顺序"),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否启用"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, comment: "并发标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DictionaryId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "字典ID", collation: "ascii_general_ci"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictionaryItem_Dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "字典选项表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    FullName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "姓名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NickName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, comment: "昵称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Avatar = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "头像")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true, comment: "出生日期"),
                    Gender = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "性别")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailVerified = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "邮箱是否验证"),
                    PhoneNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "手机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberVerified = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "手机号是否验证"),
                    PasswordHash = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, comment: "密码哈希")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "类型（字典编码：user_type）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "状态")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, comment: "并发标识")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "机构ID", collation: "ascii_general_ci"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "用户表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "令牌ID", collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "刷新令牌")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "过期时间"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "用户刷新令牌表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Dictionary",
                columns: new[] { "Id", "Code", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), "organization_type", null, null, null, null, false, "机构类型", null, null },
                    { new Guid("08daf205-1634-45d5-8195-406041db0eec"), "user_type", null, null, null, null, false, "人员类型", null, null }
                });

            migrationBuilder.InsertData(
                table: "Organization",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "IsDeleted", "IsEnabled", "Name", "Order", "ParentId", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("08da692f-4718-401c-84c5-db3341edf972"), null, null, null, false, true, "灰鲸科技", 1, null, "COMPANY", null, null },
                    { new Guid("08daf1f8-efff-4189-82f6-02184b401bbc"), null, null, null, false, true, "产品部", 1, new Guid("08da692f-4718-401c-84c5-db3341edf972"), "DEPT", null, null },
                    { new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"), null, null, null, false, true, "研发部", 2, new Guid("08da692f-4718-401c-84c5-db3341edf972"), "DEPT", null, null },
                    { new Guid("08daf20c-bde9-4d77-8a1e-6460f9a28d71"), null, null, null, false, true, "测试部", 3, new Guid("08da692f-4718-401c-84c5-db3341edf972"), "DEPT", null, null }
                });

            migrationBuilder.InsertData(
                table: "DictionaryItem",
                columns: new[] { "Id", "Code", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "DictionaryId", "IsDeleted", "IsEnabled", "Name", "Order", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"), "COMPANY", null, null, null, new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), false, true, "公司", 1, "Preset", null, null },
                    { new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"), "DEPT", null, null, null, new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), false, true, "部门", 2, "Preset", null, null },
                    { new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"), "GROUP", null, null, null, new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"), false, true, "小组", 3, "Preset", null, null },
                    { new Guid("08daf207-8abf-4fff-830f-93e48ed9a34c"), "FORMAL", null, null, null, new Guid("08daf205-1634-45d5-8195-406041db0eec"), false, true, "正式", 1, "Preset", null, null },
                    { new Guid("08daf207-c77f-4f26-88db-fc0bcd114f5c"), "INTERNSHIP", null, null, null, new Guid("08daf205-1634-45d5-8195-406041db0eec"), false, true, "实习", 2, "Preset", null, null },
                    { new Guid("08daf207-d010-4005-8d25-1aee57feb494"), "OUTSOURCING", null, null, null, new Guid("08daf205-1634-45d5-8195-406041db0eec"), false, true, "外包", 3, "Preset", null, null },
                    { new Guid("08daf207-d700-4fb6-84e7-2b059bc0840f"), "SERVICE", null, null, null, new Guid("08daf205-1634-45d5-8195-406041db0eec"), false, true, "劳务", 4, "Preset", null, null },
                    { new Guid("08daf207-df16-4a55-85d2-98590704cb8b"), "CONSULTANT", null, null, null, new Guid("08daf205-1634-45d5-8195-406041db0eec"), false, true, "顾问", 5, "Preset", null, null }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Avatar", "BirthDate", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Email", "EmailVerified", "FullName", "Gender", "IsDeleted", "NickName", "OrganizationId", "PasswordHash", "PhoneNumber", "PhoneNumberVerified", "Status", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("3a05d6f8-42ef-02da-f267-94a48964c698"), "https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png", null, null, null, null, "esofar@qq.com", false, "管理员", "Man", false, "Admin", new Guid("08da692f-4718-401c-84c5-db3341edf972"), "$2a$11$KyePw5zmuFWRBQXIFo34p.RBzJV1hiGqqVks3q6OflzJO2IlPp5uy", "13688889999", false, "Normal", "FORMAL", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Dictionary_Code",
                table: "Dictionary",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryItem_DictionaryId",
                table: "DictionaryItem",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_OrganizationId",
                table: "User",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNumber",
                table: "User",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshToken_Token",
                table: "UserRefreshToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshToken_UserId",
                table: "UserRefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryItem");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "UserRefreshToken");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Dictionary");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Organization");
        }
    }
}
