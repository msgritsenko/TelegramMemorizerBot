using Domain;
using MemorizerBot.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemorizerBot.Widgets;

internal class AddChannelWidget : BotWidget
{
    private readonly BotReplyableMessagesRepository _replyableMsgRepository;
    private readonly BotUser _user;

    public AddChannelWidget(
        BotReplyableMessagesRepository replyableMsgRepository,
        BotUserProvider userProvider,
        ITelegramBotClient botClient)
        : base(botClient)
    {
        _replyableMsgRepository = replyableMsgRepository;
        _user = userProvider.CurrentUser;
    }

    public override async Task Start()
    {
        Message sentMessage = await _botClient.SendTextMessageAsync(
              chatId: _user.ChatId,
              text: "Чтобы создать новый канал, ответьте этому сообщению название канала.");

        _replyableMsgRepository.Add(msg =>
        {
            msg.MessageId = sentMessage.MessageId;
            msg.Type = BotReplyableMessageType.NewChannnel;
        });
    }

    public override Task Callback(BotCallbackData botCallback, CallbackQuery callbackQuery)
    {
        throw new NotImplementedException();
    }
}