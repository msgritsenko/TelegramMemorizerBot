using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace MemorizerBot.Repositories;

internal class UserRepository
{
    private readonly BotDbContext _dbContext;

    public UserRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BotUser GetOrCreate(long userId)
    {
        BotUser? user = _dbContext.Users
            .Include(user => user.Channels)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null) 
        {
            user = new BotUser
            {
                Id = userId,
                Channels = new List<BotChannel>(),
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        return user;
    }
}
