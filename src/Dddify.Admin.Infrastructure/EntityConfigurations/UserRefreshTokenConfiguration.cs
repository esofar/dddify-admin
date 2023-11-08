namespace Dddify.Admin.Infrastructure.Contexts.EntityConfigurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("UserRefreshToken");
        builder.ToTable(c => c.HasComment("�û�ˢ�����Ʊ�"));

        builder.Property(c => c.Id)
            .HasComment("����ID");

        builder.Property(c => c.Token)
            .HasMaxLength(200)
            .IsRequired()
            .HasComment("ˢ������");

        builder.Property(c => c.ExpiredAt)
            .HasComment("����ʱ��");

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasComment("�û�ID");

        builder.Property(c => c.IsDeleted)
            .HasComment("�Ƿ�ɾ��");

        builder.HasIndex(c => c.Token);
    }
}