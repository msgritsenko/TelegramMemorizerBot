using Domain;
using MemorizerBot.Pages;
using MemorizerBot.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistance;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// options part
string telegramKey = string.Empty;
string sqLitePath = string.Empty;

// config part
var config = new ConfigurationBuilder()
       .AddUserSecrets<Program>()
       .Build();

if (string.IsNullOrEmpty(telegramKey))
{
    telegramKey = config["TelegramKey"]!;
}

if (string.IsNullOrEmpty(sqLitePath))
{
    sqLitePath = config["SQLitePath"];
}

// work part
var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
if (!string.IsNullOrEmpty(sqLitePath))
{
    optionsBuilder = optionsBuilder
        .UseSqlite($"Data Source={sqLitePath}");
}
else
{
    var connection = new SqliteConnection("Filename=:memory:");
    connection.Open();
    optionsBuilder = optionsBuilder
        .UseSqlite(connection);
}

using var db = new BotDbContext(optionsBuilder.Options);
db.Database.EnsureCreated();
var count = await db.Users.CountAsync();
var botClient = new TelegramBotClient(telegramKey);

using CancellationTokenSource cts = new();
var cancellationToken = cts.Token;

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

var channelRepository = new ChannelRepository(db);
var userRepository = new UserRepository(db);
var questionsRepository = new QuestionsRepository(db);

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");

Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();



async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{

    if (update.CallbackQuery is { } callback && callback.Message.MessageId > 0)
    {
        BotUser user = userRepository.GetOrCreate(callback.From.Id);

        if (callback.Data.StartsWith("WelcomePage"))
        {
            if (callback.Data == "WelcomePage;setup")
            {
                var botChannel = db.Channels.First();

                var setupPage = new SetupPage(channelRepository, user, botChannel);
                await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, true);
            }

            if (callback.Data == "WelcomePage;work")
            {
                var workPage = new WorkPage(questionsRepository, user);
                await workPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, null);
            }
        }

        if (callback.Data.StartsWith("Setup"))
        {
            if (callback.Data.StartsWith("Setup;next"))
            {
                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
                int botchannelId = int.Parse(items[2]);
                var botChannel = channelRepository.GetById(botchannelId);

                var setupPage = new SetupPage(channelRepository, user, botChannel);
                await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, true);
            }

            if (callback.Data.StartsWith("Setup;star"))
            {
                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
                int botchannelId = int.Parse(items[2]);
                var botChannel = channelRepository.GetById(botchannelId);

                user.Channels.Add(botChannel);
                db.SaveChanges();

                var setupPage = new SetupPage(channelRepository, user, botChannel);
                await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message);
            }

            if (callback.Data.StartsWith("Setup;unstar"))
            {
                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
                int botchannelId = int.Parse(items[2]);
                var botChannel = channelRepository.GetById(botchannelId);

                user.Channels.Remove(botChannel);

                db.SaveChanges();

                var setupPage = new SetupPage(channelRepository, user, botChannel);
                await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message);
            }

            if (callback.Data == "Setup;back")
            {
                var welcomePage = new WelcomePage();
                await welcomePage.ReplacePage(botClient, callback.Message.Chat, callback.Message);
            }
        }

        if (callback.Data.StartsWith("WorkPage"))
        {
            if (callback.Data.StartsWith("WorkPage;next"))
            {
                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
                int questionId = int.Parse(items[2]);
                var question = questionsRepository.GetById(questionId);

                var workPage = new WorkPage(questionsRepository, user);
                await workPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, question);
            }

            if (callback.Data.StartsWith("WorkPage;stop"))
            {
                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
                int questionId = int.Parse(items[2]);
                var question = questionsRepository.GetById(questionId);

                var workPage = new WorkPage(questionsRepository, user);
                await workPage.QuitPage(botClient, callback.Message.Chat, callback.Message, question);
            }
        }

        return;
    }

    if (update.Message is { Text: "/start" } message)
    {
        if (message.From == null || message.From.IsBot == true)
            return;

        var telegramUserId = message.From.Id;
        BotUser user = userRepository.GetOrCreate(telegramUserId);

        var welcomePage = new WelcomePage();
        await welcomePage.StartPage(botClient, message.Chat);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
