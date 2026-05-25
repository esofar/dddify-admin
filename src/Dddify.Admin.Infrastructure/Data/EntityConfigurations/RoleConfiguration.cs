using Dddify.Admin.Domain.Aggregates.Roles;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("sys_role");

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name)
            .HasMaxLength(Role.MaxNameLength)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(Role.MaxDescriptionLength);

        builder.Property(r => r.IsPreset)
            .IsRequired();

        builder.Property(r => r.IsDefault)
            .IsRequired();

        builder.Property(r => r.AssignedUserCount)
            .IsRequired();

        builder.Property(r => r.Order)
            .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.HasIndex(r => r.IsDefault);
        builder.HasIndex(r => r.Order);
    }

    private static void ConfigureSeedData(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new
            {
                Id = new Guid("018f0d8b-c694-7c32-868d-97b38f6a37c4"),
                Name = "超级管理员",
                Description = "拥有系统最高权限，管理所有模块和用户。",
                AssignedUserCount = 1,
                IsPreset = true,
                IsDefault = false,
                Order = 1,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("018f0d8b-c695-7c32-866e-f7cf8ec9fcfc"),
                Name = "管理员",
                Description = "可管理用户、角色、权限、配置等，权限略低于超级管理员。",
                AssignedUserCount = 0,
                IsPreset = true,
                IsDefault = false,
                Order = 2,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("018f0d8b-c696-7c32-8214-01bb88d69052"),
                Name = "普通用户",
                Description = "系统基础用户，使用系统提供的基本功能。",
                AssignedUserCount = 0,
                IsPreset = true,
                IsDefault = true,
                Order = 3,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("018f0d8b-c697-7c32-8675-7ec3e4eb6c9a"),
                Name = "访客",
                Description = "只读权限，不能进行任何修改操作。",
                AssignedUserCount = 0,
                IsPreset = true,
                IsDefault = false,
                Order = 4,
                IsDeleted = false,
            }
        );
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("sys_role_permission");

        ConfigureKeys(builder);
        ConfigureProperties(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }

    private static void ConfigureProperties(EntityTypeBuilder<RolePermission> builder)
    {
        builder.Property(rp => rp.RoleId)
            .IsRequired();

        builder.Property(rp => rp.PermissionId)
            .IsRequired();

        builder.Property(rp => rp.PermissionCode)
            .HasMaxLength(RolePermission.MaxPermissionCodeLength)
            .IsRequired();
    }

    private static void ConfigureIndexes(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasIndex(rp => rp.PermissionId);
        builder.HasIndex(rp => rp.PermissionCode);
    }

    private static void ConfigureSeedData(EntityTypeBuilder<RolePermission> builder)
    {
        var superAdminRoleId = Guid.Parse("018f0d8b-c694-7c32-868d-97b38f6a37c4");

        builder.HasData(
            PermissionConfiguration.SeedPermissions()
                .Select(permission => new
                {
                    RoleId = superAdminRoleId,
                    PermissionId = permission.Id,
                    PermissionCode = permission.Code,
                })
        );
    }
}
