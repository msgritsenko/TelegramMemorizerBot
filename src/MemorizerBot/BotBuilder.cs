using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MemorizerBot;

internal static class BotBuilder
{
    public static async Task<Bot> Build(
        Func<IConfiguration, string> telegramKeyExtractor,
        Action<IServiceCollection, IConfiguration> serviceCollectionBuilder,
        Action<ContainerBuilder, IConfiguration> containerBuilder,
        Action<IContainer, IConfiguration> preStartAction,
        Action<Bot> configureBotCommands,
        CancellationToken ct)
    {
        var config = new ConfigurationBuilder()
       .AddUserSecrets<Program>()
       .AddJsonFile("appsettings.json", true)
       .Build();

        string telegramKey = telegramKeyExtractor(config);

        var botClient = new TelegramBotClient(telegramKey);

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<ITelegramBotClient>(botClient);
        serviceCollection.AddScoped<BotUserProvider>();

        serviceCollectionBuilder(serviceCollection, config);

        var builder = new ContainerBuilder();
        builder.Populate(serviceCollection);

        containerBuilder(builder, config);

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        var result = new Bot()
        {
            Container = builder.Build(),
        };

        configureBotCommands(result);

        preStartAction(result.Container, config);

        var me = await botClient.GetMeAsync();

        await botClient.SetMyCommandsAsync(result.GlobalCommands.Select(c => c.Item1).ToArray());

        Console.WriteLine($"Start listening for @{me.Username}");

        botClient.StartReceiving(
            updateHandler: result.HandleUpdateAsync,
            pollingErrorHandler: result.HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: ct
        );

        return result;
    }

}
