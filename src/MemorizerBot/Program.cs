using Autofac;
using MemorizerBot;
using MemorizerBot.Repositories;
using MemorizerBot.Widgets;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
                    .UseSqlite($"Data Source={sqLitePath}");
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

        services.AddTransient<WorkWidget>();
    },
    containerBuilder: (container, cfg) =>
    {

    },
    preStartAction: (container, cfg) =>
    {
        BotDbContext db = container.Resolve<BotDbContext>();
        db.Database.EnsureCreated();
    }, 
    configureBotCommands: bot =>
    {
        var start = new BotCommand() { Command = "/start", Description = "Start memorize cards" };
        var addChannel = new BotCommand() { Command = "/addchannel", Description = "Add new channel" };
        var addCard = new BotCommand() { Command = "/addcard", Description = "Add new channel" };
        
        bot.GlobalCommands.Add((start, (bot, scope) => bot.Start<WorkWidget>(scope)));
    },
    cts.Token);

// temporary solution
bot.Widgets.Add(typeof(WorkWidget));

Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();
