using Domain;
using Persistance;

namespace MemorizerBot.Repositories;

internal class BotChannelRepository
{
    private readonly BotDbContext _dbContext;

    public BotChannelRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BotChannel GetById(int id)
    {
        return _dbContext.Channels.First(c => c.Id == id);
    }

    public BotChannel GetNext(BotChannel botChannel)
    {
        return GetNext(botChannel.Id);
    }
    public BotChannel GetNext(int botChannelId)
    {
        var channel = _dbContext.Channels
            .Where(c => c.Id > botChannelId)
            .FirstOrDefault();

        if (channel != null)
            return channel;

        return _dbContext.Channels.OrderBy(c => c.Id).First();
    }
}