using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistance;

namespace Migrations.SQLite;

public class BloggingContextFactory : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        optionsBuilder
            .UseSqlite("Data Source=D:\\p_OwnGitHubProjects\\memorizerBot.db",
            options => options
                .MigrationsAssembly(typeof(BloggingContextFactory).Assembly.FullName)
                .MigrationsHistoryTable("__EFMigrationsHistory"));

        return new BotDbContext(optionsBuilder.Options);
    }
}