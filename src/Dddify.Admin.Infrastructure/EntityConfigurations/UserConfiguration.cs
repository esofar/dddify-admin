namespace Dddify.Admin.Infrastructure.Contexts.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.ToTable(c => c.HasComment("用户表"));

        builder.Property(c => c.Id)
            .HasComment("用户ID");

        builder.Property(c => c.FullName)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("姓名");

        builder.Property(c => c.NickName)
            .HasMaxLength(20)
            .HasComment("昵称");

        builder.Property(c => c.Avatar)
            .HasMaxLength(200)
            .HasComment("头像");

        builder.Property(c => c.BirthDate)
            .HasComment("出生日期");

        builder.Property(c => c.Gender)
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired()
             .HasComment("性别");

        builder.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("邮箱");

        builder.Property(c => c.EmailVerified)
            .IsRequired()
            .HasComment("邮箱是否验证");

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("手机号");

        builder.Property(c => c.PhoneNumberVerified)
            .IsRequired()
            .HasComment("手机号是否验证");

        builder.Property(c => c.PasswordHash)
            .HasMaxLength(1000)
            .IsRequired()
            .HasComment("密码哈希");

        builder.Property(c => c.Type)
            .HasMaxLength(20)
            .IsRequired()
            .HasComment("类型（字典编码：user_type）");

        builder.Property(c => c.Status)
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired()
             .HasComment("状态");

        builder.Property(c => c.ConcurrencyStamp)
           .HasComment("并发标识");

        builder.Property(c => c.IsDeleted)
            .HasComment("是否删除");

        builder.Property(c => c.OrganizationId)
           .HasComment("机构ID");

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
                FullName = "管理员",
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