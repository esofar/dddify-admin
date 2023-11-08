namespace Dddify.Admin.Infrastructure.Contexts.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.ToTable(c => c.HasComment("�û���"));

        builder.Property(c => c.Id)
            .HasComment("�û�ID");

        builder.Property(c => c.FullName)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("����");

        builder.Property(c => c.NickName)
            .HasMaxLength(20)
            .HasComment("�ǳ�");

        builder.Property(c => c.Avatar)
            .HasMaxLength(200)
            .HasComment("ͷ��");

        builder.Property(c => c.BirthDate)
            .HasComment("��������");

        builder.Property(c => c.Gender)
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired()
             .HasComment("�Ա�");

        builder.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("����");

        builder.Property(c => c.EmailVerified)
            .IsRequired()
            .HasComment("�����Ƿ���֤");

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("�ֻ���");

        builder.Property(c => c.PhoneNumberVerified)
            .IsRequired()
            .HasComment("�ֻ����Ƿ���֤");

        builder.Property(c => c.PasswordHash)
            .HasMaxLength(1000)
            .IsRequired()
            .HasComment("�����ϣ");

        builder.Property(c => c.Type)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("���ͣ��ֵ���룺user_type��");

        builder.Property(c => c.Status)
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired()
             .HasComment("״̬");

        builder.Property(c => c.ConcurrencyStamp)
           .HasComment("������ʶ");

        builder.Property(c => c.IsDeleted)
            .HasComment("�Ƿ�ɾ��");

        builder.Property(c => c.OrganizationId)
           .HasComment("����ID");

        builder.HasOne(c => c.Organization)
            .WithMany(c => c.Users)
            .HasForeignKey(c => c.OrganizationId);

        builder.HasMany(c => c.RefreshTokens)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        builder.HasMany(c => c.Roles)
            .WithMany(c => c.Users)
            .UsingEntity<UserRole>(
                ur => ur.HasOne(c => c.Role).WithMany(c => c.UserRoles).HasForeignKey(c => c.RoleId),
                ur => ur.HasOne(c => c.User).WithMany(c => c.UserRoles).HasForeignKey(c => c.UserId),
                ur => ur.HasKey(c => new { c.UserId, c.RoleId }));

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.PhoneNumber)
            .IsUnique();

        builder.HasData(
            new
            {
                Id = new Guid("3a05d6f8-42ef-02da-f267-94a48964c698"),
                FullName = "����Ա",
                NickName = "Admin",
                Avatar = "https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png",
                Gender = UserGender.Man,
                Email = "esofar@qq.com",
                EmailVerified = false,
                PhoneNumber = "13688889999",
                PhoneNumberVerified = false,
                PasswordHash = "$2a$11$KyePw5zmuFWRBQXIFo34p.RBzJV1hiGqqVks3q6OflzJO2IlPp5uy", // default password: 'Password@2023'
                Type = "FORMAL",
                Status = UserStatus.Normal,
                OrganizationId = new Guid("08da692f-4718-401c-84c5-db3341edf972"),
                IsDeleted = false,
            });
    }
}