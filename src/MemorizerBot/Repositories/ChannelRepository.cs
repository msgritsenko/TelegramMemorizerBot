using Domain;
using Persistance;

namespace MemorizerBot.Repositories;

internal class ChannelRepository
{
    private readonly BotDbContext _dbContext;

    public ChannelRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BotChannel GetById(int id)
    {
        return _dbContext.Channels.First(c => c.Id == id);
    }

    public BotChannel GetNext(BotChannel botChannel)
    {
        var channel = _dbContext.Channels
            .Where(c => c.Id > botChannel.Id)
            .FirstOrDefault();

        if (channel != null)
            return channel;

        return _dbContext.Channels.OrderBy(c => c.Id).First();
    }
}