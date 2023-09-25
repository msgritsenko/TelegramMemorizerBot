﻿using Domain;
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

        for (int i = 0; i < 10; ++i)
        {
            int randomChannel = Random.Shared.Next(0, channels.Count);

            var channel = channels.Skip(randomChannel).FirstOrDefault();

            var question = GetNextQuestion(channel);

            if (question != null && question.Id != currentQuestionId)
            {
                return question;
            }
        }

        return null;
    }

    internal BotQuestion? GetById(int questionId)
    {
        return _dbContext.Questions
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