namespace Domain;

public enum BotReplyableMessageType
{
    NewChannnel,
    NewCard,
    ShowedCard
}

public class BotReplyableMessage
{
    public int Id {  get; set; }

    public BotReplyableMessageType Type { get; set; }

    public int Payload { get; set; }

    public string PayloadJson { get; set; }
}
