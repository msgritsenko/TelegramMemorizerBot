using Telegram.Bot;

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
    public abstract Task Callback();
}
