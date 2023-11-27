using Autofac;
using MemorizerBot;
using MemorizerBot.Repositories;
using MemorizerBot.Widgets;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migrations.SQLite;
using Persistance;
using Telegram.Bot.Types;

using CancellationTokenSource cts = new();

var bot = await BotBuilder.Build(
    telegramKeyExtractor: cfg => cfg["TelegramKey"],
    serviceCollectionBuilder: (services, cfg) =>
    {
        string? sqLitePath = cfg["SQLitePath"];

        services.AddDbContext<BotDbContext>(options =>
        {
            if (!string.IsNullOrEmpty(sqLitePath))
            {
                options = options
                    .UseSqlite($"Data Source={sqLitePath}", o => o
                        .MigrationsAssembly(typeof(BloggingContextFactory).Assembly.FullName)
                        .MigrationsHistoryTable("__EFMigrationsHistory"));
            }
            else
            {
                var connection = new SqliteConnection("Filename=:memory:");
                connection.Open();
                options = options
                    .UseSqlite(connection);
            }
        });

        services.AddScoped<BotChannelRepository>();
        services.AddScoped<BotUserRepository>();
        services.AddScoped<BotQuestionsRepository>();
        services.AddScoped<BotReplyableMessagesRepository>();

        services.AddTransient<WorkWidget>();
        services.AddTransient<AddChannelWidget>();
        services.AddTransient<SetupPage>();
    },
    containerBuilder: (container, cfg) =>
    {

    },
    preStartAction: (container, cfg) =>
    {
        BotDbContext db = container.Resolve<BotDbContext>();
        
        string? sqLitePath = cfg["SQLitePath"];

        if (!string.IsNullOrEmpty(sqLitePath))
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();
    },
    configureBotCommands: bot =>
    {
        var start = new BotCommand() { Command = "/start", Description = "Start memorize cards" };
        var addChannel = new BotCommand() { Command = "/addchannel", Description = "Add new channel" };
        var selectChannels = new BotCommand() { Command = "/selectchannels", Description = "Select favorite channels" };
        //var addCard = new BotCommand() { Command = "/addcard", Description = "Add new card" };

        bot.GlobalCommands.Add((start, (bot, scope) => bot.Start<WorkWidget>(scope)));
        bot.GlobalCommands.Add((addChannel, (bot, scope) => bot.Start<AddChannelWidget>(scope)));
        bot.GlobalCommands.Add((selectChannels, (bot, scope) => bot.Start<SetupPage>(scope)));
    },
    cts.Token);

// temporary solution
bot.Widgets.Add(typeof(WorkWidget));
bot.Widgets.Add(typeof(AddChannelWidget));
bot.Widgets.Add(typeof(SetupPage));


IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .ConfigureServices(services =>
    {
        
    })
    .UseConsoleLifetime()
    .Build();


await host.RunAsync();

