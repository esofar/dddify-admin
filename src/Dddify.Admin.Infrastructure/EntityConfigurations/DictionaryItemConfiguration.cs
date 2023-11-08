namespace Dddify.Admin.Infrastructure.EntityConfigurations;

public class DictionaryItemConfiguration : IEntityTypeConfiguration<DictionaryItem>
{
    public void Configure(EntityTypeBuilder<DictionaryItem> builder)
    {
        builder.ToTable("DictionaryItem");
        builder.ToTable(c => c.HasComment("字典选项表"));

        builder.Property(c => c.Id)
            .HasComment("字典选项ID");

        builder.Property(c => c.Code)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("编码");

        builder.Property(c => c.Name)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("名称");

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("类型");

        builder.Property(c => c.DictionaryId)
            .HasComment("字典ID");

        builder.Property(c => c.Order)
            .HasComment("显示顺序");

        builder.Property(c => c.IsEnabled)
         .HasComment("是否启用");

        builder.Property(c => c.ConcurrencyStamp)
            .HasComment("并发标识");

        builder.Property(c => c.IsDeleted)
            .HasComment("是否删除");

        builder.HasData(
            new
            {
                Id = new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"),
                DictionaryId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
                Code = "COMPANY",
                Name = "公司",
                Type = DictionaryItemType.Preset,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"),
                DictionaryId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
                Code = "DEPT",
                Name = "部门",
                Type = DictionaryItemType.Preset,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"),
                DictionaryId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
                Code = "GROUP",
                Name = "小组",
                Type = DictionaryItemType.Preset,
                Order = 3,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf207-8abf-4fff-830f-93e48ed9a34c"),
                DictionaryId = new Guid("08daf205-1634-45d5-8195-406041db0eec"),
                Code = "FORMAL",
                Name = "正式",
                Type = DictionaryItemType.Preset,
                Order = 1,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf207-c77f-4f26-88db-fc0bcd114f5c"),
                DictionaryId = new Guid("08daf205-1634-45d5-8195-406041db0eec"),
                Code = "INTERNSHIP",
                Name = "实习",
                Type = DictionaryItemType.Preset,
                Order = 2,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf207-d010-4005-8d25-1aee57feb494"),
                DictionaryId = new Guid("08daf205-1634-45d5-8195-406041db0eec"),
                Code = "OUTSOURCING",
                Name = "外包",
                Type = DictionaryItemType.Preset,
                Order = 3,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf207-d700-4fb6-84e7-2b059bc0840f"),
                DictionaryId = new Guid("08daf205-1634-45d5-8195-406041db0eec"),
                Code = "SERVICE",
                Name = "劳务",
                Type = DictionaryItemType.Preset,
                Order = 4,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf207-df16-4a55-85d2-98590704cb8b"),
                DictionaryId = new Guid("08daf205-1634-45d5-8195-406041db0eec"),
                Code = "CONSULTANT",
                Name = "顾问",
                Type = DictionaryItemType.Preset,
                Order = 5,
                IsEnabled = true,
                IsDeleted = false,
            });
    }
}