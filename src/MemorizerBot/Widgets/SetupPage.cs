using Domain;
using MemorizerBot.Repositories;
using Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemorizerBot.Widgets;

internal class SetupPage : BotWidget
{
    private readonly BotChannelRepository _channelRepository;
    private readonly BotUser _user;
    private readonly BotDbContext _dbContext;

    public SetupPage(
        BotChannelRepository channelRepository,
        BotUserProvider userProvider,
        ITelegramBotClient botClient,
        BotDbContext dbContext)
        : base(botClient)
    {
        _channelRepository = channelRepository;
        _user = userProvider.CurrentUser;
        _dbContext = dbContext;
    }

    public override async Task Start()
    {
        BotChannel nextChannel = _channelRepository.GetNext(0);

        if (nextChannel == null)
        {
            return;
        }

        bool starred = _user.Channels.Contains(nextChannel);
        var channelName = nextChannel.Name + (starred ? "  ★" : "");

        var inlineKeyboard = starred ? inlineUnstarKeyboard(nextChannel.Id) : inlineStarKeyboard(nextChannel.Id);

        Message sentMessage = await _botClient.SendTextMessageAsync(
              chatId: _user.ChatId,
              text: channelName,
              replyMarkup: inlineKeyboard);
    }

    private InlineKeyboardMarkup inlineStarKeyboard(int channelId) => new(
        new[]
        {
        InlineKeyboardButton.WithCallbackData(text: "стоп", callbackData: BuildCallBack(nameof(Quit), channelId)),
        InlineKeyboardButton.WithCallbackData(text: "следить", callbackData: BuildCallBack(nameof(Star), channelId)),
        InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: BuildCallBack(nameof(Next), channelId)),
        }
    );

    private InlineKeyboardMarkup inlineUnstarKeyboard(int channelId) => new(
        new[]
        {
        InlineKeyboardButton.WithCallbackData(text: "стоп", callbackData: BuildCallBack(nameof(Quit), channelId)),
        InlineKeyboardButton.WithCallbackData(text: "забыть", callbackData: BuildCallBack(nameof(UnStar), channelId)),
        InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: BuildCallBack(nameof(Next), channelId)),
        }
    );

    public override Task Callback(BotCallbackData botCallback, CallbackQuery callbackQuery)
    {
        return botCallback.Action switch
        {
            nameof(Quit) => Quit(botCallback.GetPayload<int>(), callbackQuery),
            nameof(Next) => Next(botCallback.GetPayload<int>(), callbackQuery),
            nameof(Star) => Star(botCallback.GetPayload<int>(), callbackQuery),
            nameof(UnStar) => UnStar(botCallback.GetPayload<int>(), callbackQuery),

            _ => Task.CompletedTask,
        };
    }

    private async Task Next(int currentQuestionId, CallbackQuery callbackQuery)
    {
        BotChannel nextChannel = _channelRepository.GetNext(currentQuestionId);

        bool starred = _user.Channels.Contains(nextChannel);
        var channelName = nextChannel.Name + (starred ? "  ★" : "");

        var inlineKeyboard = starred ? inlineUnstarKeyboard(nextChannel.Id) : inlineStarKeyboard(nextChannel.Id);

        Message sentMessage = await _botClient.EditMessageTextAsync(
               chatId: _user.ChatId,
               messageId: callbackQuery.Message.MessageId,

               text: channelName,
               replyMarkup: inlineKeyboard);
    }

    private async Task Quit(int currentQuestionId, CallbackQuery callbackQuery)
    {
        BotChannel nextChannel = _channelRepository.GetById(currentQuestionId);

        bool starred = _user.Channels.Contains(nextChannel);
        var channelName = nextChannel.Name + (starred ? "  ★" : "");

        Message sentMessage = await _botClient.EditMessageTextAsync(
               chatId: _user.ChatId,
               messageId: callbackQuery.Message.MessageId,

               text: channelName,
               replyMarkup: null);
    }

    private async Task Star(int currentQuestionId, CallbackQuery callbackQuery)
    {
        var botChannel = _channelRepository.GetById(currentQuestionId);

        _user.Channels.Add(botChannel);
        await _dbContext.SaveChangesAsync();

        BotChannel nextChannel = _channelRepository.GetById(currentQuestionId);

        bool starred = _user.Channels.Contains(nextChannel);
        var channelName = nextChannel.Name + (starred ? "  ★" : "");

        var inlineKeyboard = starred ? inlineUnstarKeyboard(nextChannel.Id) : inlineStarKeyboard(nextChannel.Id);

        Message sentMessage = await _botClient.EditMessageTextAsync(
               chatId: _user.ChatId,
               messageId: callbackQuery.Message.MessageId,

               text: channelName,
               replyMarkup: inlineKeyboard);
    }

    private async Task UnStar(int currentQuestionId, CallbackQuery callbackQuery)
    {
        var botChannel = _channelRepository.GetById(currentQuestionId);

        _user.Channels.Remove(botChannel);
        await _dbContext.SaveChangesAsync();

        await Next(currentQuestionId, callbackQuery);

        BotChannel nextChannel = _channelRepository.GetById(currentQuestionId);

        bool starred = _user.Channels.Contains(nextChannel);
        var channelName = nextChannel.Name + (starred ? "  ★" : "");

        var inlineKeyboard = starred ? inlineUnstarKeyboard(nextChannel.Id) : inlineStarKeyboard(nextChannel.Id);

        Message sentMessage = await _botClient.EditMessageTextAsync(
               chatId: _user.ChatId,
               messageId: callbackQuery.Message.MessageId,

               text: channelName,
               replyMarkup: inlineKeyboard);
    }
}