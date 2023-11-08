namespace Dddify.Admin.Infrastructure.Contexts.EntityConfigurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("UserRefreshToken");
        builder.ToTable(c => c.HasComment("用户刷新令牌表"));

        builder.Property(c => c.Id)
            .HasComment("令牌ID");

        builder.Property(c => c.Token)
            .HasMaxLength(200)
            .IsRequired()
            .HasComment("刷新令牌");

        builder.Property(c => c.ExpiredAt)
            .HasComment("过期时间");

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasComment("用户ID");

        builder.Property(c => c.IsDeleted)
            .HasComment("是否删除");

        builder.HasIndex(c => c.Token);
    }
}