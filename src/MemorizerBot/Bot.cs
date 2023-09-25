using Autofac;
using Domain;
using MemorizerBot.Repositories;
using MemorizerBot.Widgets;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MemorizerBot;

internal class Bot
{
    public List<(BotCommand, Func<Bot, ILifetimeScope, Task>)> GlobalCommands { get; set; } = new List<(BotCommand, Func<Bot, ILifetimeScope, Task>)>();

    public List<Type> Widgets { get; set; } = new List<Type>();

    public IContainer Container { get; set; }

    public Bot() { }

    public async Task Start<TWidget>(ILifetimeScope scope) where TWidget : BotWidget
    {
        // current scope
        var widget = scope.Resolve<TWidget>();
        await widget.Start();
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

            BotCallbackData botCallBack = BotCallbackData.FromJsonString(callback.Data);

            var widgetType = Widgets.First(w => w.Name == botCallBack.WidgetName);
            var widget = scope.Resolve(widgetType) as BotWidget;

            await widget.Callback(botCallBack, callback);

            return;
        }

        if (update.Message is { } message)
        {
            if (message.From == null || message.From.IsBot == true)
                return;


            if (message.ReplyToMessage is { } replyMessage)
            {
                using var scope = Container.BeginLifetimeScope();
                // ----
                var replyableRepository = scope.Resolve<BotReplyableMessagesRepository>();
                var questionsRepository = scope.Resolve<BotQuestionsRepository>();

                var db = scope.Resolve<BotDbContext>();

                var originalMsg = replyableRepository.Get(replyMessage.MessageId);

                if (originalMsg.Type == BotReplyableMessageType.ShowedCard)
                {
                    var question = questionsRepository.GetById(originalMsg.Payload);
                    question.Query = message.Text;

                    await db.SaveChangesAsync();
                }

                if (originalMsg.Type == BotReplyableMessageType.NewChannnel)
                {
                    if (!await db.Channels.AnyAsync(c => c.Name == message.Text))
                    {
                        await db.Channels.AddAsync(new BotChannel { Name = message.Text });

                        await db.SaveChangesAsync();
                    }
                }

                return;
            }


            var commandExists = GlobalCommands.Any(c => c.Item1.Command.Equals(message.Text, StringComparison.Ordinal));
            if (commandExists == false)
            {
                return;
            }
            else
            {
                (BotCommand cmd, Func<Bot, ILifetimeScope, Task> action) = GlobalCommands.First(c => c.Item1.Command.Equals(message.Text, StringComparison.Ordinal));

                using var scope = Container.BeginLifetimeScope();
                // ----
                var userRepository = scope.Resolve<BotUserRepository>();
                var userProvider = scope.Resolve<BotUserProvider>();

                BotUser user = userRepository.GetOrCreate(message.From.Id, message.Chat.Id);
                userProvider.CurrentUser = user;
                // ----

                await action(this, scope);
            }
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
