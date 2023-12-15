using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace MemorizerBot.Repositories;

internal class BotQuestionsRepository
{
    private readonly BotDbContext _dbContext;

    public BotQuestionsRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BotQuestion? GetNextQuestion(IReadOnlyCollection<BotChannel> channels, int currentQuestionId)
    {
        if (channels.Count == 0)
            return null;

        var channelIds = channels.Select(i => i.Id).ToList();

        for (int i = 0; i < 10; ++i)
        {
            var question = _dbContext.Questions
                .Include(q => q.Channel)
                .Where(q => channelIds.Contains(q.BotChannelid))
                .Where(q => q.Id > currentQuestionId)
                .OrderBy(q => q.Id)
                .FirstOrDefault();

            if (question == null)
                currentQuestionId = -1;
            else
                return question;
        }

        return null;
    }

    internal BotQuestion? GetById(int questionId)
    {
        return _dbContext.Questions
            .Include(c => c.Channel)
            .Where(c => c.Id == questionId)
            .FirstOrDefault();
    }

    private BotQuestion? GetNextQuestion(BotChannel channel)
    {
        var count = _dbContext.Questions
            .Where(c => c.BotChannelid == channel.Id)
            .Count();

        int randomQuery = Random.Shared.Next(0, count);
        var result = _dbContext.Questions
            .Where(c => c.BotChannelid == channel.Id)
            .Skip(randomQuery)
            .FirstOrDefault();

        return result;
    }
}