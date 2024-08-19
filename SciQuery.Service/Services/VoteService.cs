using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.Interfaces;

namespace SciQuery.Service.Services;

public class VoteService(SciQueryDbContext dbContext,IReputationService reputationService) : IVoteService
{
    private readonly SciQueryDbContext _dbContext = dbContext;
    private readonly IReputationService _reputationService = reputationService;

    public async Task DownVote(string userId, int postId, PostType postType)
    {
        if (postType is PostType.Question)
        {
            await VoteQuestion(postId, -1);
            await _reputationService.DownVotedQuestionReputation(userId);
        }
        else if (postType is PostType.Answer)
        {
            await VoteAnswer(postId, -1);
            await _reputationService.DownVotedAnswerReputation(userId);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpVote(string userId, int postId,PostType postType)
    {
        if (postType is PostType.Question)
        {
            await VoteQuestion(postId, 1);
            await _reputationService.UpVotedQuestionReputation(userId);
        }
        else if (postType is PostType.Answer)
        {
            await VoteAnswer(postId, 1);
            await _reputationService.UpVotedAnswerReputation(userId);
        }
        await _dbContext.SaveChangesAsync();
    }
    private async Task VoteQuestion(int postId,int point)
    {
        var post = await _dbContext.Questions.FirstOrDefaultAsync(c => c.Id == postId)
                ?? throw new EntityNotFoundException();
        post.Votes += point;
        _dbContext.Update(post);

    }
    private async Task VoteAnswer(int postId, int point)
    {
        var post = await _dbContext.Questions.FirstOrDefaultAsync(c => c.Id == postId)
               ?? throw new EntityNotFoundException();
        post.Votes += point;
        _dbContext.Update(post);
    }
}
