using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class BotQuestionConfiguration : IEntityTypeConfiguration<BotQuestion>
{
    public void Configure(EntityTypeBuilder<BotQuestion> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Query)
            .IsRequired();

        builder
            .Property(x => x.Answer);

        builder
            .HasOne<BotChannel>()
            .WithMany()
            .HasForeignKey(x => x.BotChannelid);
    }
}