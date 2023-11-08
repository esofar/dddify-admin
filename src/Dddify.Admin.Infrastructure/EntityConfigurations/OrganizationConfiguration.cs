namespace Dddify.Admin.Infrastructure.Contexts.EntityConfigurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organization");
        builder.ToTable(c => c.HasComment("机构表"));

        builder.Property(c => c.Id)
            .HasComment("机构ID");

        builder.Property(c => c.ParentId)
            .HasComment("上级ID");

        builder.Property(c => c.Name)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("名称");

        builder.Property(c => c.Type)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("类型（字典编码：organization_type）");

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
                Id = new Guid("08da692f-4718-401c-84c5-db3341edf972"),
                DictionaryId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
                Name = "灰鲸科技",
                Type = "COMPANY",
                Order = 1,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf1f8-efff-4189-82f6-02184b401bbc"),
                ParentId = new Guid("08da692f-4718-401c-84c5-db3341edf972"),
                Name = "产品部",
                Type = "DEPT",
                Order = 1,
                IsEnabled = true,
                IsDeleted = false,
            },
            new
            {
                Id = new Guid("08daf1f8-f887-4a99-8e8c-fed496abb4f7"),
                ParentId = new Guid("08da692f-4718-401c-84c5-db3341edf972"),
                Name = "研发部",
                Type = "DEPT",
                Order = 2,
                IsEnabled = true,
                IsDeleted = false,
            }, new
            {
                Id = new Guid("08daf20c-bde9-4d77-8a1e-6460f9a28d71"),
                ParentId = new Guid("08da692f-4718-401c-84c5-db3341edf972"),
                Name = "测试部",
                Type = "DEPT",
                Order = 3,
                IsEnabled = true,
                IsDeleted = false,
            });
    }
}