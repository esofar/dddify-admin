using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dddify.Admin.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAnnouncementEnumsToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE sys_announcement
                ALTER COLUMN status TYPE character varying(20)
                USING CASE status
                    WHEN 1 THEN 'Draft'
                    WHEN 2 THEN 'Published'
                    WHEN 3 THEN 'Withdrawn'
                    ELSE status::text
                END;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE sys_announcement
                ALTER COLUMN audience_type TYPE character varying(20)
                USING CASE audience_type
                    WHEN 1 THEN 'AllUsers'
                    WHEN 2 THEN 'Departments'
                    WHEN 3 THEN 'Roles'
                    WHEN 4 THEN 'SpecificUsers'
                    ELSE audience_type::text
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE sys_announcement
                ALTER COLUMN status TYPE integer
                USING CASE status
                    WHEN 'Draft' THEN 1
                    WHEN 'Published' THEN 2
                    WHEN 'Withdrawn' THEN 3
                    ELSE status::integer
                END;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE sys_announcement
                ALTER COLUMN audience_type TYPE integer
                USING CASE audience_type
                    WHEN 'AllUsers' THEN 1
                    WHEN 'Departments' THEN 2
                    WHEN 'Roles' THEN 3
                    WHEN 'SpecificUsers' THEN 4
                    ELSE audience_type::integer
                END;
                """);
        }
    }
}
