using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class BotReplyableMessageConfiguration : IEntityTypeConfiguration<BotReplyableMessage>
{
    public void Configure(EntityTypeBuilder<BotReplyableMessage> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Type)
            .HasConversion<string>();

        builder
            .Property(x => x.MessageId);

        builder
            .HasIndex(x => x.MessageId)
            .IsUnique();
    }
}
