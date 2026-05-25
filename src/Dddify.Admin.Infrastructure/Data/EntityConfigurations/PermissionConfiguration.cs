using Dddify.Admin.Domain.Aggregates.Permissions;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("sys_permission");

        ConfigureProperties(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(p => p.Id);

        builder.Property(p => p.ParentId);

        builder.Property(p => p.Code)
            .HasMaxLength(Permission.MaxCodeLength)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(Permission.MaxNameLength)
            .IsRequired();

        builder.Property(p => p.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Order)
            .IsRequired();
    }

    private static void ConfigureSeedData(EntityTypeBuilder<Permission> builder)
    {
        builder.HasData(SeedPermissions());
    }

    internal static IEnumerable<Permission> SeedPermissions()
    {
        var systemId = Guid.Parse("018f69e2-55a3-7c7b-8c52-4d1e31f69b25");
        var userId = Guid.Parse("018f69e2-55a4-7c7b-a56d-b16c37e51952");
        var roleId = Guid.Parse("018f69e2-55a5-7c7b-a7a6-c2b2beabc408");
        var permissionId = Guid.Parse("018f69e2-55a6-7c7b-818b-93ea29100998");
        var departmentId = Guid.Parse("018f69e2-55a7-7c7b-9c64-c9f62a84ec7b");
        var lookupId = Guid.Parse("018f69e2-55a8-7c7b-80b2-55d4a1216a89");

        return
        [
            new(systemId, null, "system", "系统管理", PermissionType.Catalog, 1),

            new(userId, systemId, "system:user:index", "用户管理", PermissionType.Menu, 1),
            new(new Guid("01979d24-ea82-708b-997a-1311dfba482b"), userId, "system:user:create", "新增用户", PermissionType.Button, 2),
            new(new Guid("01979d24-ea82-70cf-950c-32cc434d18f2"), userId, "system:user:update", "编辑用户", PermissionType.Button, 3),
            new(new Guid("01979d24-ea82-7286-b804-cea0cf28a01a"), userId, "system:user:delete", "删除用户", PermissionType.Button, 4),
            new(new Guid("01979d24-ea82-73e6-9e25-5004391febce"), userId, "system:user:enable", "启用用户", PermissionType.Button, 5),
            new(new Guid("01979d24-ea82-748d-bd21-c8faee1b6642"), userId, "system:user:disable", "禁用用户", PermissionType.Button, 6),
            new(new Guid("01979d24-ea82-7493-a171-cebf1e23c98c"), userId, "system:user:reset-password", "重置密码", PermissionType.Button, 7),
            new(new Guid("01979d24-ea82-749e-939a-c43c0a851959"), userId, "system:user:assign-roles", "分配角色", PermissionType.Button, 8),

            new(roleId, systemId, "system:role:index", "角色管理", PermissionType.Menu, 9),
            new(new Guid("01979d24-ea82-74c0-9a62-49ca0cbfe12b"), roleId, "system:role:create", "新增角色", PermissionType.Button, 10),
            new(new Guid("01979d24-ea82-7536-8d50-c1b5bc023503"), roleId, "system:role:update", "编辑角色", PermissionType.Button, 11),
            new(new Guid("01979d24-ea82-75a5-b394-e39d25d01626"), roleId, "system:role:delete", "删除角色", PermissionType.Button, 12),
            new(new Guid("01979d24-ea82-75ee-b48c-205c452262bb"), roleId, "system:role:assign-permissions", "权限配置", PermissionType.Button, 13),

            new(permissionId, systemId, "system:permission:index", "权限管理", PermissionType.Menu, 14),
            new(new Guid("01979d24-ea82-7687-b31a-46586630b624"), permissionId, "system:permission:create", "新增权限", PermissionType.Button, 15),
            new(new Guid("01979d24-ea82-77ea-abc3-a15b515e5b80"), permissionId, "system:permission:update", "编辑权限", PermissionType.Button, 16),
            new(new Guid("01979d24-ea82-780e-814c-e62cc6680b3a"), permissionId, "system:permission:delete", "删除权限", PermissionType.Button, 17),

            new(departmentId, systemId, "system:department:index", "部门管理", PermissionType.Menu, 18),
            new(new Guid("01979d24-ea82-7858-b328-feb7c02db5c7"), departmentId, "system:department:create", "新增部门", PermissionType.Button, 19),
            new(new Guid("01979d24-ea82-79ce-9036-7793446abb1b"), departmentId, "system:department:update", "编辑部门", PermissionType.Button, 20),
            new(new Guid("01979d24-ea82-7ac3-aabb-23ba54940dc9"), departmentId, "system:department:delete", "删除部门", PermissionType.Button, 21),

            new(lookupId, systemId, "system:lookup:index", "字典管理", PermissionType.Menu, 22),
            new(new Guid("01979d24-ea82-7ae3-a345-b896e6392195"), lookupId, "system:lookup:create", "新增字典", PermissionType.Button, 23),
            new(new Guid("01979d24-ea82-7b63-ad5e-9d8e917d3e5c"), lookupId, "system:lookup:update", "编辑字典", PermissionType.Button, 24),
            new(new Guid("01979d24-ea82-7c14-bc58-18f42c8be279"), lookupId, "system:lookup:delete", "删除字典", PermissionType.Button, 25),
            new(new Guid("01979d24-ea82-7c77-9c92-be1b4413c498"), lookupId, "system:lookup:item:create", "新增字典项", PermissionType.Button, 26),
            new(new Guid("01979d24-ea82-7cf9-a242-88bd19882f78"), lookupId, "system:lookup:item:update", "修改字典项", PermissionType.Button, 27),
            new(new Guid("01979d24-ea82-7d29-ab3e-aab3d1c996df"), lookupId, "system:lookup:item:disable", "禁用字典项", PermissionType.Button, 28),
            new(new Guid("019a075c-8b2e-7f8d-a678-95c158f2fc6d"), lookupId, "system:lookup:item:enable", "启用字典项", PermissionType.Button, 29),
            new(new Guid("019a48f8-d4c7-7049-bb57-15e38c00f979"), lookupId, "system:lookup:item:delete", "删除字典项", PermissionType.Button, 30),
        ];
    }
}
