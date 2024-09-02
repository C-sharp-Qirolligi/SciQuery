using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.Interfaces;

namespace SciQuery.Service.Services;

public class ReputationService(SciQueryDbContext dbContext) : IReputationService
{
    private readonly SciQueryDbContext _dbContext = dbContext;

    public async Task AcceptedAnswerReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new EntityNotFoundException();

        user.Reputation += 200;
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateAnswerReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId) 
            ?? throw new EntityNotFoundException();
        
        user.Reputation += 100;
        await _dbContext.SaveChangesAsync();   
    }

    public async Task CreateQuestionReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new EntityNotFoundException();

        user.Reputation += 50;
        await _dbContext.SaveChangesAsync();

    }

    public async Task DownVotedAnswerReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
          ?? throw new EntityNotFoundException();

        user.Reputation -= 50;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DownVotedQuestionReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new EntityNotFoundException();

        user.Reputation -= 30;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpVotedAnswerReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new EntityNotFoundException();

        user.Reputation += 100;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpVotedQuestionReputation(string userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new EntityNotFoundException();

        user.Reputation += 50;
        await _dbContext.SaveChangesAsync(); ;
    }
}
