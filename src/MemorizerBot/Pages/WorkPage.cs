using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Domain;
using Telegram.Bot.Types;
using MemorizerBot.Repositories;

namespace MemorizerBot.Pages;

internal class WorkPage
{
    private readonly QuestionsRepository _questionsRepository;
    private readonly BotUser _user;

    public WorkPage(QuestionsRepository questionsRepository, BotUser user)
    {
        _questionsRepository = questionsRepository;
        _user = user;
    }

    public async Task ReplacePage(ITelegramBotClient botClient, Chat chat, Message message, BotQuestion? botQuestion)
    {
        BotQuestion? question = _questionsRepository.GetNextQuestion(_user.Channels);

        while (question?.Id == botQuestion?.Id)
        {
            if (question == null)
                return;
            question = _questionsRepository.GetNextQuestion(_user.Channels);
        }

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "стоп", callbackData: $"WorkPage;stop;{question.Id}"),
                InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: $"WorkPage;next;{question.Id}"),
            },
        });
        
        try
        {
            Message sentMessage = await botClient.EditMessageTextAsync(
                chatId: chat.Id,
                messageId: message.MessageId,

                text: question.Query,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: inlineKeyboard);

        }
        catch (Exception)
        {
        }
    }

    public async Task QuitPage(ITelegramBotClient botClient, Chat chat, Message message, BotQuestion question)
    {
        try
        {
            Message sentMessage = await botClient.EditMessageTextAsync(
                chatId: chat.Id,
                messageId: message.MessageId,

                text: question.Query,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: null);
        }
        catch (Exception)
        {
        }
    }
}
