using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemorizerBot.Pages;

internal class WelcomePage
{
    public async Task StartPage(ITelegramBotClient botClient, Chat chat)
    {
        try
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chat.Id,
                text: text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                replyMarkup: inlineKeyboard);
        }
        catch (Exception)
        {
        }
    }

    public async Task ReplacePage(ITelegramBotClient botClient, Chat chat, Message message)
    {
        try
        {
            Message sentMessage = await botClient.EditMessageTextAsync(
                chatId: chat.Id,
                messageId: message.MessageId,
                text: text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                replyMarkup: inlineKeyboard);
        }
        catch (Exception)
        {
        }
    }

    const string text = @"Приветствуем *Вас* на канале для разработчиков
Настройте интересующие вас каналы и пользуйтесь с удовольствием";

    private InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "настройка", callbackData: "WelcomePage;setup"),
                InlineKeyboardButton.WithCallbackData(text: "использование", callbackData: "WelcomePage;work"),
            }
        );
}
