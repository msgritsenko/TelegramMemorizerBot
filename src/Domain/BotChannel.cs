namespace Domain;

public record BotChannel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<BotUser> Users { get; } = new();
}