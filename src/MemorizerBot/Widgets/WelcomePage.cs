//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.ReplyMarkups;

//namespace MemorizerBot.Widgets;

//internal class WelcomePage
//{
//    private readonly ITelegramBotClient _botClient;

//    public WelcomePage(ITelegramBotClient botClient)
//    {
//        _botClient = botClient;
//    }

//    public async Task StartPage(Chat chat)
//    {
//        try
//        {
//            Message sentMessage = await _botClient.SendTextMessageAsync(
//                chatId: chat.Id,
//                text: text,
//                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
//                replyMarkup: inlineKeyboard);
//        }
//        catch (Exception)
//        {
//        }
//    }

//    public async Task ReplacePage(ITelegramBotClient botClient, Chat chat, Message message)
//    {
//        try
//        {
//            Message sentMessage = await botClient.EditMessageTextAsync(
//                chatId: chat.Id,
//                messageId: message.MessageId,
//                text: text,
//                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
//                replyMarkup: inlineKeyboard);
//        }
//        catch (Exception)
//        {
//        }
//    }

//    const string text = @"Приветствуем *Вас* на канале для разработчиков
//Настройте интересующие вас каналы и пользуйтесь с удовольствием";

//    private InlineKeyboardMarkup inlineKeyboard = new(
//            new[]
//            {
//            InlineKeyboardButton.WithCallbackData(text: "настройка", callbackData: "WelcomePage;setup"),
//            InlineKeyboardButton.WithCallbackData(text: "использование", callbackData: "WelcomePage;work"),
//            }
//        );
//}
//using MemorizerBot.Repositories;
//using MemorizerBot.Widgets;
//using Telegram.Bot.Types;

//if (callback.Data.StartsWith("WelcomePage"))
//{
//    if (callback.Data == "WelcomePage;setup")
//    {
//        var botChannel = db.Channels.First();

//        var setupPage = new SetupPage(channelRepository, user, botChannel);
//        await setupPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, true);
//    }

//    if (callback.Data == "WelcomePage;work")
//    {
//        var workPage = new WorkPage(questionsRepository, user);
//        await workPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, null);
//    }
//}