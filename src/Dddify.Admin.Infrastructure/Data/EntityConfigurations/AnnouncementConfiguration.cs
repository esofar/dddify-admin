using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("sys_announcement");

        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<Announcement> builder)
    {
        builder.Property(a => a.Title)
            .HasMaxLength(Announcement.MaxTitleLength)
            .IsRequired();

        builder.Property(a => a.Summary)
            .HasMaxLength(Announcement.MaxSummaryLength)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Announcement> builder)
    {
        builder.OwnsOne(a => a.Content, content =>
        {
            content.Property(c => c.Html)
                .IsRequired();

            content.Property(c => c.PlainText)
                .IsRequired();
        });

        builder.OwnsOne(a => a.Audience, audience =>
        {
            audience.Property(a => a.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            audience.Property(a => a.TargetIds)
                .IsRequired();
        });
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasIndex(a => new { a.IsDeleted, a.CreatedAt });

        builder.HasIndex(a => new { a.IsDeleted, a.Status, a.CreatedAt });
    }
}
