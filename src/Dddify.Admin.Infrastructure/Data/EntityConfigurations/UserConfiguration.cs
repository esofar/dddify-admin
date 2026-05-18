using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("sys_user");

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name)
            .HasMaxLength(User.MaxNameLength)
            .IsRequired();

        builder.Property(u => u.NickName)
            .HasMaxLength(User.MaxNickNameLength);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(User.MaxPasswordHashLength)
            .IsRequired();

        builder.Property(u => u.Avatar)
            .HasMaxLength(User.MaxAvatarLength);

        builder.Property(u => u.Email)
            .HasMaxLength(User.MaxEmailLength)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(User.MaxPhoneNumberLength)
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.IsBuiltIn)
            .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(u => u.Department, department =>
        {
            department.Property(d => d.Id)
                .IsRequired();

            department.Property(d => d.Name)
                .HasMaxLength(UserDepartment.MaxNameLength)
                .IsRequired();

            department.HasIndex(d => d.Id);
        });

        builder.HasMany(u => u.Roles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();

        builder.HasIndex(u => u.Gender);
        builder.HasIndex(u => u.Status);
    }

    private static void ConfigureSeedData(EntityTypeBuilder<User> builder)
    {
        var defaultPasswordHash = "$2a$11$PzXBLYTFLe2dprKajYFXcO92917uRAC.zSzakWQubHTcxlqjpHHKm"; // Admin123

        var systemAdminUserId = Guid.Parse("3a05d6f8-42ef-02da-f267-94a48964c698");
        var operationsAdminUserId = Guid.Parse("c1e9475a-59d7-4a3c-b68d-8c144f403a51");
        var productLeadUserId = Guid.Parse("eb426312-7e53-4a0b-98df-2c1a4a4c432e");
        var developmentLeadUserId = Guid.Parse("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4");
        var qaLeadUserId = Guid.Parse("c55c35a1-84a2-419a-8db8-93b1c3b02bb9");
        var auditorUserId = Guid.Parse("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1");
        var rootDepartmentId = Guid.Parse("08da692f-4718-401c-84c5-db3341edf972");
        var productDepartmentId = Guid.Parse("08daf1f8-efff-4189-82f6-02184b401bbc");
        var technologyCenterId = Guid.Parse("08daf1f8-f887-4a99-8e8c-fed496abb4f7");
        var functionalCenterId = Guid.Parse("392a90cc-da28-4535-bf7b-cceb25938c4b");
        var userGrowthGroupId = Guid.Parse("6c2b4d3f-755e-4b69-a16c-12906a9015e7");
        var qualityDepartmentId = Guid.Parse("08daf20c-bde9-4d77-8a1e-6460f9a28d71");

        builder.HasData(
            new
            {
                Id = systemAdminUserId,
                Name = "顾承远",
                NickName = "Chengyuan Gu",
                PasswordHash = defaultPasswordHash,
                Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=chengyuan-gu",
                Gender = UserGender.Male,
                Status = UserStatus.Enabled,
                Email = "chengyuan.gu@xinghan.tech",
                PhoneNumber = "13800138001",
                IsBuiltIn = true,
                IsDeleted = false
            },
            new
            {
                Id = operationsAdminUserId,
                Name = "唐雨桐",
                NickName = "Yutong Tang",
                PasswordHash = defaultPasswordHash,
                Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=yutong-tang",
                Gender = UserGender.Female,
                Status = UserStatus.Enabled,
                Email = "yutong.tang@xinghan.tech",
                PhoneNumber = "13800138002",
                IsBuiltIn = false,
                IsDeleted = false
            },
            new
            {
                Id = productLeadUserId,
                Name = "沈知夏",
                NickName = "Zhixia Shen",
                PasswordHash = defaultPasswordHash,
                Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=zhixia-shen",
                Gender = UserGender.Female,
                Status = UserStatus.Enabled,
                Email = "zhixia.shen@xinghan.tech",
                PhoneNumber = "13800138003",
                IsBuiltIn = false,
                IsDeleted = false
            },
            new
            {
                Id = developmentLeadUserId,
                Name = "陆景行",
                NickName = "Jingxing Lu",
                PasswordHash = defaultPasswordHash,
                Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=jingxing-lu",
                Gender = UserGender.Male,
                Status = UserStatus.Enabled,
                Email = "jingxing.lu@xinghan.tech",
                PhoneNumber = "13800138004",
                IsBuiltIn = false,
                IsDeleted = false
            },
            new
            {
                Id = qaLeadUserId,
                Name = "林若宁",
                NickName = "Ruoning Lin",
                PasswordHash = defaultPasswordHash,
                Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=ruoning-lin",
                Gender = UserGender.Female,
                Status = UserStatus.Enabled,
                Email = "ruoning.lin@xinghan.tech",
                PhoneNumber = "13800138005",
                IsBuiltIn = false,
                IsDeleted = false
            },
            new
            {
                Id = auditorUserId,
                Name = "韩叙白",
                NickName = "Xubai Han",
                PasswordHash = defaultPasswordHash,
                Avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=xubai-han",
                Gender = UserGender.Male,
                Status = UserStatus.Enabled,
                Email = "xubai.han@xinghan.tech",
                PhoneNumber = "13800138006",
                IsBuiltIn = false,
                IsDeleted = false
            });

        builder.OwnsOne(u => u.Department).HasData(
            new { UserId = systemAdminUserId, Id = rootDepartmentId, Name = "星瀚科技集团" },
            new { UserId = operationsAdminUserId, Id = userGrowthGroupId, Name = "用户增长组" },
            new { UserId = productLeadUserId, Id = productDepartmentId, Name = "产品中心" },
            new { UserId = developmentLeadUserId, Id = technologyCenterId, Name = "技术中心" },
            new { UserId = qaLeadUserId, Id = qualityDepartmentId, Name = "测试质量部" },
            new { UserId = auditorUserId, Id = functionalCenterId, Name = "职能中心" });
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    private static readonly Guid AdminUserId = Guid.Parse("3a05d6f8-42ef-02da-f267-94a48964c698");
    private static readonly Guid OperationsAdminUserId = Guid.Parse("c1e9475a-59d7-4a3c-b68d-8c144f403a51");
    private static readonly Guid ProductLeadUserId = Guid.Parse("eb426312-7e53-4a0b-98df-2c1a4a4c432e");
    private static readonly Guid DevelopmentLeadUserId = Guid.Parse("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4");
    private static readonly Guid QaLeadUserId = Guid.Parse("c55c35a1-84a2-419a-8db8-93b1c3b02bb9");
    private static readonly Guid AuditorUserId = Guid.Parse("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1");
    private static readonly Guid SuperAdminRoleId = Guid.Parse("018f0d8b-c694-7c32-868d-97b38f6a37c4");
    private static readonly Guid UserRoleId = Guid.Parse("018f0d8b-c696-7c32-8214-01bb88d69052");

    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("sys_user_role");

        ConfigureKeys(builder);
        ConfigureProperties(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }

    private static void ConfigureProperties(EntityTypeBuilder<UserRole> builder)
    {
        builder.Property(ur => ur.UserId)
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .IsRequired();

        builder.Property(ur => ur.RoleName)
            .HasMaxLength(UserRole.MaxRoleNameLength)
            .IsRequired();

        builder.Property(ur => ur.IsCurrent)
            .IsRequired();
    }

    private static void ConfigureIndexes(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasIndex(ur => ur.RoleId);
    }

    private static void ConfigureSeedData(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(
            new
            {
                UserId = AdminUserId,
                RoleId = SuperAdminRoleId,
                RoleName = "系统管理员",
                IsCurrent = true
            },
            new
            {
                UserId = AdminUserId,
                RoleId = UserRoleId,
                RoleName = "普通用户",
                IsCurrent = false
            },
            new
            {
                UserId = OperationsAdminUserId,
                RoleId = UserRoleId,
                RoleName = "普通用户",
                IsCurrent = true
            },
            new
            {
                UserId = ProductLeadUserId,
                RoleId = UserRoleId,
                RoleName = "普通用户",
                IsCurrent = true
            },
            new
            {
                UserId = DevelopmentLeadUserId,
                RoleId = UserRoleId,
                RoleName = "普通用户",
                IsCurrent = true
            },
            new
            {
                UserId = QaLeadUserId,
                RoleId = UserRoleId,
                RoleName = "普通用户",
                IsCurrent = true
            },
            new
            {
                UserId = AuditorUserId,
                RoleId = UserRoleId,
                RoleName = "普通用户",
                IsCurrent = true
            });
    }
}
