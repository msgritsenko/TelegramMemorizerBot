using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistance;

public class BotDbContext : DbContext
{
    public DbSet<BotUser> Users { get; set; } = null!;
    public DbSet<BotChannel> Channels { get; set; } = null!;
    public DbSet<BotQuestion> Questions { get; set; } = null!;
    public DbSet<BotReplyableMessage> ReplyableMessages { get; set; } = null!;

    public BotDbContext(DbContextOptions<BotDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }
}