using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemorizerBot.Widgets;

internal abstract class BotWidget
{
    protected readonly ITelegramBotClient _botClient;

    public BotWidget(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    /// <summary>
    /// Starts a new message.
    /// </summary>
    public abstract Task Start();
    
    /// <summary>
    /// Edits an existing message.
    /// </summary>
    public abstract Task Callback(BotCallbackData botCallback, CallbackQuery callbackQuery);

    protected string BuildCallBack<T>(string action, T payload)
    {
        var botCallback = BotCallbackData.Build(this.GetType().Name, action, payload);

        return botCallback.ToJsonString();
    }
}
