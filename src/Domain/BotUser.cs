namespace Domain;

public class BotUser
{
    public long Id { get; set; }

    public long ChatId { get; set; }

    public List<BotChannel> Channels { get; set; } = new();
}