using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class BotChannelConfiguration : IEntityTypeConfiguration<BotChannel>
{
    public void Configure(EntityTypeBuilder<BotChannel> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Name)
            .IsRequired();

        builder
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}