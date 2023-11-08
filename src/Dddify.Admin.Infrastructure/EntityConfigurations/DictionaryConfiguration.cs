namespace Dddify.Admin.Infrastructure.EntityConfigurations;

public class DictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
{
    public void Configure(EntityTypeBuilder<Dictionary> builder)
    {
        builder.ToTable("Dictionary");
        builder.ToTable(c => c.HasComment("字典表"));

        builder.Property(c => c.Id)
            .HasComment("字典ID");

        builder.Property(c => c.Code)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("编码");

        builder.Property(c => c.Name)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("名称");

        builder.Property(c => c.Description)
          .HasMaxLength(150)
          .HasComment("描述");

        builder.Property(c => c.ConcurrencyStamp)
            .HasComment("并发标识");

        builder.Property(c => c.IsDeleted)
            .HasComment("是否删除");

        builder.HasMany(t => t.Items)
            .WithOne(t => t.Dictionary)
            .HasForeignKey(t => t.DictionaryId);

        builder.HasIndex(c => c.Code);

        builder.HasData(
            new
            {
                Id = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
                Code = "organization_type",
                Name = "机构类型",
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf205-1634-45d5-8195-406041db0eec"),
                Code = "user_type",
                Name = "人员类型",
                IsDeleted = false,
            });
    }
}