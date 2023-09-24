using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace MemorizerBot.Repositories;

internal class BotUserRepository
{
    private readonly BotDbContext _dbContext;

    public BotUserRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BotUser Get(long userId)
    {
        BotUser user = _dbContext.Users
            .Include(user => user.Channels)
            .Single(u => u.Id == userId);

        return user;
    }

    public BotUser GetOrCreate(long userId, long chatId)
    {
        BotUser? user = _dbContext.Users
            .Include(user => user.Channels)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            user = new BotUser
            {
                Id = userId,
                ChatId = chatId,
                Channels = new List<BotChannel>(),
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        if (user.ChatId == 0)
        {
            user.ChatId = chatId;
            _dbContext.SaveChanges();
        }

        return user;
    }
}