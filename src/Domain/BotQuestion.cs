namespace Domain;

public class BotQuestion
{
    public int Id { get; set; }
    public string Query { get; set; }
    public string? Answer { get; set; }

    public int BotChannelid { get; set; }
}