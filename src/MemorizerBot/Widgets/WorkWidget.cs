using Domain;
using MemorizerBot.Repositories;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemorizerBot.Widgets;

internal class WorkWidget : BotWidget
{
    private readonly QuestionsRepository _questionsRepository;
    private readonly BotUser _user;

    public WorkWidget(
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
        BotQuestion? question = _questionsRepository.GetNextQuestion(_user.Channels, 0);

        if (question == null)
        {
            // нет ни вопросов ни каналов - сообщить пользователю что надо выбрать каналы
            return;
        }

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "стоп", callbackData: BuildCallBack(nameof(Quit), question.Id)),
                InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: BuildCallBack(nameof(Next), question.Id)),
            },
        });

        Message sentMessage = await _botClient.SendTextMessageAsync(
              chatId: _user.ChatId,
              text: question.Query,
              //parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
              replyMarkup: inlineKeyboard);
    }

    public override Task Callback(BotCallback botCallback, CallbackQuery callbackQuery)
    {
        return botCallback.Action switch
        {
            nameof(Quit) => Quit(botCallback.GetPayload<int>(), callbackQuery),
            nameof(Next) => Next(botCallback.GetPayload<int>(), callbackQuery),

            _ => Task.CompletedTask,
        };
    }

    private async Task Next(int currentQuestionId, CallbackQuery callbackQuery)
    {
        var question = _questionsRepository.GetNextQuestion(_user.Channels, currentQuestionId);

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "стоп", callbackData: BuildCallBack(nameof(Quit), question.Id)),
                InlineKeyboardButton.WithCallbackData(text: "дальше", callbackData: BuildCallBack(nameof(Next), question.Id)),
            },
        });

        Message sentMessage = await _botClient.EditMessageTextAsync(
               chatId: _user.ChatId,
               messageId: callbackQuery.Message.MessageId,

               text: question.Query,
               //parseMode: Telegram.Bot.Types.Enums.ParseMode.te,
               replyMarkup: inlineKeyboard);
    }

    private async Task Quit(int currentQuestionId, CallbackQuery callbackQuery)
    {
        var lastQuestion = _questionsRepository.GetById(currentQuestionId);


        Message sentMessage = await _botClient.EditMessageTextAsync(
               chatId: _user.ChatId,
               messageId: callbackQuery.Message.MessageId,

               text: lastQuestion.Query,
               //parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
               replyMarkup: null);
    }
}