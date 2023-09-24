using Domain;
using MemorizerBot.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemorizerBot.Widgets;

internal class WorkWindget : BotWidget
{
    private readonly QuestionsRepository _questionsRepository;
    private readonly BotUser _user;

    public WorkWindget(
        QuestionsRepository questionsRepository,
        BotUserProvider userProvider,
        ITelegramBotClient botClient)
        : base(botClient)
    {
        _questionsRepository = questionsRepository;
        _user = userProvider.CurrentUser;
    }

    public override async Task Start()
    {
        // получить новый вопрос
        // сформировать навигационные меню
        // отправить сообщение
        BotQuestion? question = _questionsRepository.GetNextQuestion(_user.Channels);

        if (question == null)
        {
            // нет ни вопросов ни каналов - сообщить пользователю что надо выбрать каналы
            return;
        }

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "стоп", callbackData: $"WorkPage;stop;{question.Id}"),
                InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: $"WorkPage;next;{question.Id}"),
            },
        });

        Message sentMessage = await _botClient.SendTextMessageAsync(
              chatId: _user.ChatId,
              text: question.Query,
              parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
              replyMarkup: inlineKeyboard);
    }

    public override async Task Callback()
    {
        // восстановить состояние из payload
        // сформировать вопрос
        // отредактировать сообщение

        try
        {
            //Message sentMessage = await _botClient.EditMessageTextAsync(
            //    chatId: chat.Id,
            //    messageId: message.MessageId,

            //    text: question.Query,
            //    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            //    replyMarkup: inlineKeyboard);

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

//     if (callback.Data.StartsWith("WorkPage"))
//        {
//            if (callback.Data.StartsWith("WorkPage;next"))
//            {
//                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
//    int questionId = int.Parse(items[2]);
//    var question = questionsRepository.GetById(questionId);

//    var workPage = new WorkPage(questionsRepository, user);
//    await workPage.ReplacePage(botClient, callback.Message.Chat, callback.Message, question);
//}

//            if (callback.Data.StartsWith("WorkPage;stop"))
//            {
//                var items = callback.Data.Split(';', StringSplitOptions.RemoveEmptyEntries);
//int questionId = int.Parse(items[2]);
//var question = questionsRepository.GetById(questionId);

//var workPage = new WorkPage(questionsRepository, user);
//await workPage.QuitPage(botClient, callback.Message.Chat, callback.Message, question);
//            }
//        }
//}