namespace Dddify.Admin.Infrastructure.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.ToTable(c => c.HasComment("角色表"));

        builder.Property(c => c.Id)
            .HasComment("角色ID");

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
    }
}