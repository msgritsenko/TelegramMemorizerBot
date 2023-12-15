using Telegram.Bot.Types;

namespace Domain;

public class BotQuestion
{
    public int Id { get; set; }
    public string Query { get; set; }
    public int BotChannelid { get; set; }
    public BotChannel Channel { get; set; }

    public List<MessageEntity> Entities { get; set; }
}