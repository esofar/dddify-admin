namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

using SessionEntity = Domain.Aggregates.Sessions.Session;

public class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.ToTable("sys_session");

        ConfigureProperties(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.DeviceId)
            .HasMaxLength(SessionEntity.MaxDeviceIdLength)
            .IsRequired();

        builder.Property(s => s.DeviceName)
            .HasMaxLength(SessionEntity.MaxDeviceNameLength)
            .IsRequired();

        builder.Property(s => s.IpAddress)
            .HasMaxLength(SessionEntity.MaxIpAddressLength);

        builder.Property(s => s.UserAgent)
            .HasMaxLength(SessionEntity.MaxUserAgentLength);

        builder.Property(s => s.RefreshTokenHash)
            .HasMaxLength(SessionEntity.MaxRefreshTokenHashLength)
            .IsRequired();

        builder.Property(s => s.PreviousRefreshTokenHash)
            .HasMaxLength(SessionEntity.MaxRefreshTokenHashLength);

        builder.Property(s => s.IsPersistent)
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .IsRequired();

        builder.Property(s => s.LastSeenAt);
        builder.Property(s => s.RevokedAt);

        builder.Property(s => s.RevokedReason)
            .HasMaxLength(SessionEntity.MaxRevokedReasonLength);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => new { s.UserId, s.DeviceId });

        builder.HasIndex(s => s.RefreshTokenHash)
            .IsUnique();

        builder.HasIndex(s => s.PreviousRefreshTokenHash);
        builder.HasIndex(s => s.ExpiresAt);
        builder.HasIndex(s => s.RevokedAt);
    }
}
