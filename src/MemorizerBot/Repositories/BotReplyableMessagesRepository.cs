using Domain;
using Persistance;

namespace MemorizerBot.Repositories;

internal class BotReplyableMessagesRepository
{
    private readonly BotDbContext _dbContext;

    public BotReplyableMessagesRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BotReplyableMessage Get(int messageId)
    {
        BotReplyableMessage message = _dbContext.ReplyableMessages
            .Single(m => m.MessageId == messageId);

        return message;
    }

    public bool Update(int messageId, Action<BotReplyableMessage> update)
    {
        var msg = _dbContext.ReplyableMessages
            .FirstOrDefault(m => m.MessageId == messageId);

        if (msg == null)
        {
            return false;
        }

        update(msg);
        
        return _dbContext.SaveChanges() > 0;
    }

    public void Add(Action<BotReplyableMessage> update)
    {
        var msg = new BotReplyableMessage();
        update(msg);

        _dbContext.Add(msg);
        _dbContext.SaveChanges();
    }
}