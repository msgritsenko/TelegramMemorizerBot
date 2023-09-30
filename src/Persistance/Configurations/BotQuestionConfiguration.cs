using System.Text.Json;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telegram.Bot.Types;

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
            .Property(x => x.Entities)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrEmpty(v) ? new List <MessageEntity>() : JsonSerializer.Deserialize<List<MessageEntity>>(v, (JsonSerializerOptions)null))
            ;
        
        builder
            .HasOne<BotChannel>()
            .WithMany()
            .HasForeignKey(x => x.BotChannelid);
    }
}