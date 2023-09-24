using Autofac;
using Domain;
using MemorizerBot.Repositories;
using MemorizerBot.Widgets;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MemorizerBot;

internal class Bot
{
    public List<(BotCommand, Action<Bot, ILifetimeScope>)> GlobalCommands { get; set; } = new List<(BotCommand, Action<Bot, ILifetimeScope>)>();

    public List<Type> Widgets { get; set; } = new List<Type>();

    public IContainer Container { get; set; }

    public Bot() { }

    public void Start<TWidget>(ILifetimeScope scope) where TWidget : BotWidget
    {
        // current scope
        var widget = scope.Resolve<TWidget>();
        widget.Start();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.CallbackQuery is { } callback)
        {
            using var scope = Container.BeginLifetimeScope();
            // ----
            var userRepository = scope.Resolve<BotUserRepository>();
            var userProvider = scope.Resolve<BotUserProvider>();

            BotUser user = userRepository.Get(callback.From.Id);
            userProvider.CurrentUser = user;
            // ----

            BotCallback botCallBack = BotCallback.FromJsonString(callback.Data);
            
            var widgetType = Widgets.First(w => w.Name == botCallBack.WidgetName);
            var widget = scope.Resolve(widgetType) as BotWidget;
            
            await widget.Callback(botCallBack, callback);

            return;
        }

        if (update.Message is { } message)
        {
            if (message.From == null || message.From.IsBot == true)
                return;

            var commandExists = GlobalCommands.Any(c => c.Item1.Command.Equals(message.Text, StringComparison.Ordinal));
            if (commandExists == false)
            {
                return;
            }

            (BotCommand cmd, Action<Bot, ILifetimeScope> action) = GlobalCommands.First(c => c.Item1.Command.Equals(message.Text, StringComparison.Ordinal));

            using var scope = Container.BeginLifetimeScope();
            // ----
            var userRepository = scope.Resolve<BotUserRepository>();
            var userProvider = scope.Resolve<BotUserProvider>();

            BotUser user = userRepository.GetOrCreate(message.From.Id, message.Chat.Id);
            userProvider.CurrentUser = user;
            // ----

            action(this, scope);
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
}
