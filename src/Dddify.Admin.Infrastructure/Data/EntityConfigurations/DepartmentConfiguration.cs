using Dddify.Admin.Domain.Aggregates.Departments;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("sys_department");

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Department> builder)
    {
        builder.Property(d => d.Name)
            .HasMaxLength(Department.MaxNameLength)
            .IsRequired();

        builder.Property(d => d.Code)
            .HasMaxLength(Department.MaxCodeLength)
            .IsRequired();

        builder.Property(d => d.FullName)
            .HasMaxLength(Department.MaxFullNameLength)
            .IsRequired();

        builder.Property(d => d.Path)
            .HasMaxLength(Department.MaxPathLength)
            .IsRequired();

        builder.Property(d => d.Level)
            .IsRequired();

        builder.Property(d => d.Type)
            .HasMaxLength(Department.MaxTypeLength)
            .IsRequired();

        builder.Property(d => d.IsEnabled)
            .IsRequired();

        builder.Property(d => d.Order)
            .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Department> builder)
    {
        builder.OwnsOne(d => d.Leader, leader =>
        {
            leader.Property(p => p.Id)
                .IsRequired();

            leader.Property(p => p.Name)
                .HasMaxLength(DepartmentLeader.MaxNameLength)
                .IsRequired();
        });
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Department> builder)
    {
        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.HasIndex(d => new { d.ParentId, d.Name })
            .IsUnique();

        builder.HasIndex(d => d.Path);
        builder.HasIndex(d => d.Level);
        builder.HasIndex(d => new { d.IsDeleted, d.Order });
        builder.HasIndex(d => new { d.IsDeleted, d.Type, d.IsEnabled, d.Order });
        builder.HasIndex(d => new { d.ParentId, d.IsDeleted, d.Order });
    }

    private static void ConfigureSeedData(EntityTypeBuilder<Department> builder)
    {
        var rootId = Guid.Parse("08da692f-4718-401c-84c5-db3341edf972");
        var productDepartmentId = Guid.Parse("08daf1f8-efff-4189-82f6-02184b401bbc");
        var technologyCenterId = Guid.Parse("08daf1f8-f887-4a99-8e8c-fed496abb4f7");
        var functionalCenterId = Guid.Parse("392a90cc-da28-4535-bf7b-cceb25938c4b");
        var aiProductGroupId = Guid.Parse("430aaead-e559-41b0-9079-cf3875f06004");
        var userGrowthGroupId = Guid.Parse("6c2b4d3f-755e-4b69-a16c-12906a9015e7");
        var platformDevelopmentId = Guid.Parse("48c316ea-ed0c-4e4e-b2fb-83ab11a78dad");
        var infrastructureDepartmentId = Guid.Parse("282d1497-7d7a-4029-86f0-6ac935010e4f");
        var qualityDepartmentId = Guid.Parse("08daf20c-bde9-4d77-8a1e-6460f9a28d71");
        var financeDepartmentId = Guid.Parse("ff54b97f-d686-495e-9e1c-f0dc9249720e");
        var humanResourceDepartmentId = Guid.Parse("3ad207b4-762c-42b3-acae-62f3665db5df");
        var administrationDepartmentId = Guid.Parse("93a9e788-7357-4afe-9fbe-8477a74e904f");

        var rootCode = "Pk2d2L";
        var productDepartmentCode = "fWY0m0";
        var technologyCenterCode = "tt7y07";
        var functionalCenterCode = "HhN4o9";
        var aiProductGroupCode = "TJj7qx";
        var userGrowthGroupCode = "ybQKAw";
        var platformDevelopmentCode = "88H8OW";
        var infrastructureDepartmentCode = "8iI3CK";
        var qualityDepartmentCode = "JZC2A1";
        var financeDepartmentCode = "Jd7S04";
        var humanResourceDepartmentCode = "dJFLWU";
        var administrationDepartmentCode = "ctgmoD";

        builder.HasData(
            new
            {
                Id = rootId,
                ParentId = (Guid?)null,
                Name = "星瀚科技集团",
                FullName = "星瀚科技集团",
                Type = "headquarters",
                Level = 0,
                Path = $"/{rootCode}/",
                Code = rootCode,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = productDepartmentId,
                ParentId = rootId,
                Name = "产品中心",
                FullName = "星瀚科技集团/产品中心",
                Type = "business",
                Level = 1,
                Path = $"/{rootCode}/{productDepartmentCode}/",
                Code = productDepartmentCode,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = technologyCenterId,
                ParentId = rootId,
                Name = "技术中心",
                FullName = "星瀚科技集团/技术中心",
                Type = "business",
                Level = 1,
                Path = $"/{rootCode}/{technologyCenterCode}/",
                Code = technologyCenterCode,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = functionalCenterId,
                ParentId = rootId,
                Name = "职能中心",
                FullName = "星瀚科技集团/职能中心",
                Type = "business",
                Level = 1,
                Path = $"/{rootCode}/{functionalCenterCode}/",
                Code = functionalCenterCode,
                Order = 3,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = aiProductGroupId,
                ParentId = productDepartmentId,
                Name = "AI 产品组",
                FullName = "星瀚科技集团/产品中心/AI 产品组",
                Type = "product",
                Level = 2,
                Path = $"/{rootCode}/{productDepartmentCode}/{aiProductGroupCode}/",
                Code = aiProductGroupCode,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = userGrowthGroupId,
                ParentId = productDepartmentId,
                Name = "用户增长组",
                FullName = "星瀚科技集团/产品中心/用户增长组",
                Type = "product",
                Level = 2,
                Path = $"/{rootCode}/{productDepartmentCode}/{userGrowthGroupCode}/",
                Code = userGrowthGroupCode,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = platformDevelopmentId,
                ParentId = technologyCenterId,
                Name = "平台研发部",
                FullName = "星瀚科技集团/技术中心/平台研发部",
                Type = "technology",
                Level = 2,
                Path = $"/{rootCode}/{technologyCenterCode}/{platformDevelopmentCode}/",
                Code = platformDevelopmentCode,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = infrastructureDepartmentId,
                ParentId = technologyCenterId,
                Name = "基础架构部",
                FullName = "星瀚科技集团/技术中心/基础架构部",
                Type = "technology",
                Level = 2,
                Path = $"/{rootCode}/{technologyCenterCode}/{infrastructureDepartmentCode}/",
                Code = infrastructureDepartmentCode,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = qualityDepartmentId,
                ParentId = technologyCenterId,
                Name = "测试质量部",
                FullName = "星瀚科技集团/技术中心/测试质量部",
                Type = "technology",
                Level = 2,
                Path = $"/{rootCode}/{technologyCenterCode}/{qualityDepartmentCode}/",
                Code = qualityDepartmentCode,
                Order = 3,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = financeDepartmentId,
                ParentId = functionalCenterId,
                Name = "财务部",
                FullName = "星瀚科技集团/职能中心/财务部",
                Type = "finance",
                Level = 2,
                Path = $"/{rootCode}/{functionalCenterCode}/{financeDepartmentCode}/",
                Code = financeDepartmentCode,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = humanResourceDepartmentId,
                ParentId = functionalCenterId,
                Name = "人力资源部",
                FullName = "星瀚科技集团/职能中心/人力资源部",
                Type = "human_resource",
                Level = 2,
                Path = $"/{rootCode}/{functionalCenterCode}/{humanResourceDepartmentCode}/",
                Code = humanResourceDepartmentCode,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = administrationDepartmentId,
                ParentId = functionalCenterId,
                Name = "行政部",
                FullName = "星瀚科技集团/职能中心/行政部",
                Type = "administration",
                Level = 2,
                Path = $"/{rootCode}/{functionalCenterCode}/{administrationDepartmentCode}/",
                Code = administrationDepartmentCode,
                Order = 3,
                IsEnabled = true,
                IsDeleted = false
            }
        );

        builder.OwnsOne(d => d.Leader).HasData(
            new { DepartmentId = rootId, Id = Guid.Parse("3a05d6f8-42ef-02da-f267-94a48964c698"), Name = "顾承远" },
            new { DepartmentId = productDepartmentId, Id = Guid.Parse("eb426312-7e53-4a0b-98df-2c1a4a4c432e"), Name = "沈知夏" },
            new { DepartmentId = technologyCenterId, Id = Guid.Parse("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), Name = "陆景行" },
            new { DepartmentId = functionalCenterId, Id = Guid.Parse("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), Name = "韩叙白" },
            new { DepartmentId = aiProductGroupId, Id = Guid.Parse("eb426312-7e53-4a0b-98df-2c1a4a4c432e"), Name = "沈知夏" },
            new { DepartmentId = userGrowthGroupId, Id = Guid.Parse("c1e9475a-59d7-4a3c-b68d-8c144f403a51"), Name = "唐雨桐" },
            new { DepartmentId = platformDevelopmentId, Id = Guid.Parse("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), Name = "陆景行" },
            new { DepartmentId = infrastructureDepartmentId, Id = Guid.Parse("f2c5e5a3-3e25-4b23-b1cd-bcbcb0e8f5c4"), Name = "陆景行" },
            new { DepartmentId = qualityDepartmentId, Id = Guid.Parse("c55c35a1-84a2-419a-8db8-93b1c3b02bb9"), Name = "林若宁" },
            new { DepartmentId = financeDepartmentId, Id = Guid.Parse("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), Name = "韩叙白" },
            new { DepartmentId = humanResourceDepartmentId, Id = Guid.Parse("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), Name = "韩叙白" },
            new { DepartmentId = administrationDepartmentId, Id = Guid.Parse("6d44d1c2-c92b-4056-bd8e-8b7531d9f9a1"), Name = "韩叙白" }
        );
    }
}
