using Dddify.Admin.Domain.Aggregates.Lookups;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class LookupConfiguration : IEntityTypeConfiguration<Lookup>
{
    public void Configure(EntityTypeBuilder<Lookup> builder)
    {
        builder.ToTable("sys_lookup");

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Lookup> builder)
    {
        builder.Property(d => d.Code)
            .HasMaxLength(Lookup.MaxCodeLength)
            .IsRequired();

        builder.Property(d => d.Name)
            .HasMaxLength(Lookup.MaxNameLength)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(Lookup.MaxDescriptionLength);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Lookup> builder)
    {
        builder.HasMany(u => u.Items)
            .WithOne()
            .HasForeignKey(c => c.LookupId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Lookup> builder)
    {
        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.HasIndex(d => d.Name)
            .IsUnique();
    }

    private static void ConfigureSeedData(EntityTypeBuilder<Lookup> builder)
    {
        builder.HasData(
            new
            {
                Id = new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88"),
                Code = "department_type",
                Name = "部门类型",
                IsDeleted = false,
            });
    }
}

public class LookupItemConfiguration : IEntityTypeConfiguration<LookupItem>
{
    public void Configure(EntityTypeBuilder<LookupItem> builder)
    {
        builder.ToTable("sys_lookup_item");

        ConfigureProperties(builder);
        ConfigureIndexes(builder);
        ConfigureSeedData(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<LookupItem> builder)
    {
        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.LookupId)
            .IsRequired();

        builder.Property(i => i.Value)
            .HasMaxLength(LookupItem.MaxValueLength)
            .IsRequired();

        builder.Property(i => i.Label)
            .HasMaxLength(LookupItem.MaxLabelLength)
            .IsRequired();

        builder.Property(i => i.Color)
            .HasMaxLength(LookupItem.MaxColorLength);

        builder.Property(i => i.Order)
            .IsRequired();

        builder.Property(i => i.IsPreset)
            .IsRequired();

        builder.Property(i => i.IsEnabled)
            .IsRequired();
    }

    private static void ConfigureIndexes(EntityTypeBuilder<LookupItem> builder)
    {
        builder.HasIndex(i => new { i.LookupId, i.Value })
            .IsUnique();

        builder.HasIndex(i => new { i.LookupId, i.Order });
    }

    private static void ConfigureSeedData(EntityTypeBuilder<LookupItem> builder)
    {
        var lookupId = new Guid("019e3b16-b3e0-7a3e-9c01-845320b46a88");

        builder.HasData(
            new
            {
                Id = new Guid("019e3b16-b41b-7780-bc9d-e46e02359074"),
                LookupId = lookupId,
                Value = "headquarters",
                Label = "总部",
                IsPreset = true,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41b-7905-9176-d3bbcce11e83"),
                LookupId = lookupId,
                Value = "business",
                Label = "业务部门",
                IsPreset = true,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41c-77b0-8471-5b98ea7cc354"),
                LookupId = lookupId,
                Value = "product",
                Label = "产品部门",
                IsPreset = true,
                Order = 3,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41c-75c8-bbd2-64834915b320"),
                LookupId = lookupId,
                Value = "technology",
                Label = "技术部门",
                IsPreset = true,
                Order = 4,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41c-7722-b9d9-2046045e36c3"),
                LookupId = lookupId,
                Value = "operations",
                Label = "运营部门",
                IsPreset = true,
                Order = 5,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41d-72c7-a4a0-6428d89d6df4"),
                LookupId = lookupId,
                Value = "marketing",
                Label = "市场部门",
                IsPreset = true,
                Order = 6,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41d-7c9c-a2af-0c0d41a1eb89"),
                LookupId = lookupId,
                Value = "sales",
                Label = "销售部门",
                IsPreset = true,
                Order = 7,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41f-75a1-9786-4ed862324a37"),
                LookupId = lookupId,
                Value = "customer_service",
                Label = "客服部门",
                IsPreset = true,
                Order = 8,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b41f-759e-aed3-e723ce06a88d"),
                LookupId = lookupId,
                Value = "finance",
                Label = "财务部门",
                IsPreset = true,
                Order = 9,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b420-7300-9645-db31c3391eef"),
                LookupId = lookupId,
                Value = "human_resource",
                Label = "人力资源部",
                IsPreset = true,
                Order = 10,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b420-76f4-8e33-62fae4848a01"),
                LookupId = lookupId,
                Value = "administration",
                Label = "行政部门",
                IsPreset = true,
                Order = 11,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b421-76a0-801d-5001e52b6fcc"),
                LookupId = lookupId,
                Value = "legal",
                Label = "法务部门",
                IsPreset = true,
                Order = 12,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b421-7105-bb22-f13c4594419e"),
                LookupId = lookupId,
                Value = "security",
                Label = "安全部门",
                IsPreset = true,
                Order = 13,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b422-73c2-a178-ee95103ec424"),
                LookupId = lookupId,
                Value = "procurement",
                Label = "采购部门",
                IsPreset = true,
                Order = 14,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b423-77d6-8892-d7d83a8ff17c"),
                LookupId = lookupId,
                Value = "audit",
                Label = "审计部门",
                IsPreset = true,
                Order = 15,
                IsEnabled = true,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("019e3b16-b423-79c1-a37c-7524ae11a8d7"),
                LookupId = lookupId,
                Value = "branch",
                Label = "分公司",
                IsPreset = true,
                Order = 16,
                IsEnabled = true,
                IsDeleted = false
            });
    }
}
