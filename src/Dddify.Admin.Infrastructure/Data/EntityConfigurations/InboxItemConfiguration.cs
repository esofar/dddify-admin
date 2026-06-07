using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Infrastructure.Data.EntityConfigurations;

public class InboxItemConfiguration : IEntityTypeConfiguration<InboxItem>
{
    public void Configure(EntityTypeBuilder<InboxItem> builder)
    {
        builder.ToTable("sys_inbox_item");

        ConfigureProperties(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureProperties(EntityTypeBuilder<InboxItem> builder)
    {
        builder.Property(i => i.UserId)
            .IsRequired();

        builder.Property(i => i.Title)
            .HasMaxLength(InboxItem.MaxTitleLength)
            .IsRequired();

        builder.Property(i => i.Summary)
            .HasMaxLength(InboxItem.MaxSummaryLength)
            .IsRequired();

        builder.OwnsOne(i => i.Source, source =>
        {
            source.Property(s => s.Type)
                .IsRequired();

            source.Property(s => s.Id)
                .IsRequired();

            source.HasIndex(s => new { s.Type, s.Id });
        });

        builder.Property(i => i.IsRead)
            .IsRequired();
    }

    private static void ConfigureIndexes(EntityTypeBuilder<InboxItem> builder)
    {
        builder.HasIndex(i => new { i.UserId, i.IsDeleted, i.CreatedAt });

        builder.HasIndex(i => new { i.UserId, i.IsDeleted, i.IsRead, i.CreatedAt });
    }
}
