using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Domain;
using MemorizerBot.Repositories;
using MemorizerBot.Widgets;

namespace MemorizerBot.Widgets;

internal class SetupPage
{
    private readonly BotChannelRepository _channelRepository;
    private readonly BotUser _user;
    private readonly BotChannel _currentChannel;

    public SetupPage(BotChannelRepository channelRepository, BotUser user, BotChannel current)
    {
        _channelRepository = channelRepository;
        _user = user;
        _currentChannel = current;
    }

    public async Task ReplacePage(ITelegramBotClient botClient, Chat chat, Message message, bool gotoNext = false)
    {
        BotChannel nextChannel = gotoNext ? _channelRepository.GetNext(_currentChannel) : _currentChannel;

        bool starred = _user.Channels.Contains(nextChannel);

        var inlineKeyboard = starred ? inlineUnstarKeyboard(nextChannel.Id) : inlineStarKeyboard(nextChannel.Id);
        var channelName = nextChannel.Name + (starred ? "  ★" : "");

        try
        {
            Message sentMessage = await botClient.EditMessageTextAsync(
                chatId: chat.Id,
                messageId: message.MessageId,
                text: channelName, //MarkdownHelpers.ToMarkdown(channelName),
                                   //parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            replyMarkup: inlineKeyboard);
        }
        catch (Exception)
        {
        }
    }

    private InlineKeyboardMarkup inlineStarKeyboard(int channelId) => new(
        new[]
        {
        InlineKeyboardButton.WithCallbackData(text: "назад", callbackData: "Setup;back"),
        InlineKeyboardButton.WithCallbackData(text: "следить", callbackData: $"Setup;star;{channelId}"),
        InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: $"Setup;next;{channelId}"),
        }
    );

    private InlineKeyboardMarkup inlineUnstarKeyboard(int channelId) => new(
        new[]
        {
        InlineKeyboardButton.WithCallbackData(text: "назад", callbackData: "Setup;back"),
        InlineKeyboardButton.WithCallbackData(text: "забыть", callbackData: $"Setup;unstar;{channelId}"),
        InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: $"Setup;next;{channelId}"),
        }
    );
}

//    if (callback.Data.StartsWith("Setup"))
//        {
//            if (callback.Data.StartsWith("Setup;next"))
//            {
//                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
//    int botchannelId = int.Parse(items[2]);
//    var botChannel = channelRepository.GetById(botchannelId);

//    var setupPage = new SetupPage(channelRepository, user, botChannel);
//    await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, true);
//}

//            if (callback.Data.StartsWith("Setup;star"))
//            {
//                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
//int botchannelId = int.Parse(items[2]);
//var botChannel = channelRepository.GetById(botchannelId);

//user.Channels.Add(botChannel);
//                db.SaveChanges();

//                var setupPage = new SetupPage(channelRepository, user, botChannel);
//await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message);
//            }

//            if (callback.Data.StartsWith("Setup;unstar"))
//{
//    var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
//    int botchannelId = int.Parse(items[2]);
//    var botChannel = channelRepository.GetById(botchannelId);

//    user.Channels.Remove(botChannel);

//    db.SaveChanges();

//    var setupPage = new SetupPage(channelRepository, user, botChannel);
//    await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message);
//}

//if (callback.Data == "Setup;back")
//{
//    var welcomePage = new WelcomePage();
//    await welcomePage.ReplacePage(botClient, callback.Message.Chat, callback.Message);
//}
//        }
//}