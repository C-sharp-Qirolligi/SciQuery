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

    public async Task<(bool,string)> DownVote(string userId, int postId, PostType postType)
    {
        bool check = false;

        if(postType is PostType.Answer)
        {
            check = await CheckUserHasVotedAnswer(userId, postId);
        }
        else if(postType is PostType.Question)
        {
            check = await CheckUserHasVotedQuestion(userId, postId);
        }

        if (check)
        {
            return (false, "You voted already!");
        }

        if (postType is PostType.Question)
        {
            await VoteQuestion(userId,postId, -1);
            await _reputationService.DownVotedQuestionReputation(userId);
        }
        else if (postType is PostType.Answer)
        {
            await VoteAnswer(userId,postId, -1);
            await _reputationService.DownVotedAnswerReputation(userId);
        }
        await _dbContext.SaveChangesAsync();

        return (true, "Corrextly!");
    }

    public async Task<(bool,string)> UpVote(string userId, int postId,PostType postType)
    {
        bool check = false;
        if (postType is PostType.Answer)
        {
            check = await CheckUserHasVotedAnswer(userId, postId);
        }
        else if (postType is PostType.Question)
        {
            check = await CheckUserHasVotedQuestion(userId, postId);
        }

        if (check)
        {
            return (false, "You voted already!");
        }

        if (postType is PostType.Question)
        {
            await VoteQuestion(userId,postId, 1);
            await _reputationService.UpVotedQuestionReputation(userId);
        }
        else if (postType is PostType.Answer)
        {
            await VoteAnswer(userId,postId, 1);
            await _reputationService.UpVotedAnswerReputation(userId);
        }
        await _dbContext.SaveChangesAsync();

        return (true, "Correctly!");
    }
    private async Task VoteQuestion(string userId,int postId,int point)
    {
        var post = await _dbContext.Questions.FirstOrDefaultAsync(c => c.Id == postId)
                ?? throw new EntityNotFoundException();
        post.Votes += point;

        post.VotedUsersIds ??= new();

        post.VotedUsersIds.Add(userId);

        _dbContext.Update(post);

    }
    private async Task VoteAnswer(string userId, int postId, int point)
    {
        var post = await _dbContext.Answers.FirstOrDefaultAsync(c => c.Id == postId)
               ?? throw new EntityNotFoundException();
        post.Votes += point;

        post.VotedUsersIds ??= new();
        
        post.VotedUsersIds.Add(userId);

        _dbContext.Update(post);
    }
    private async Task<bool> CheckUserHasVotedAnswer(string userId,int postId)
    {
        var post = await _dbContext.Answers.AsNoTracking().FirstOrDefaultAsync(x => x.Id ==  postId)
            ?? throw new EntityNotFoundException();

        return post.VotedUsersIds != null && post.VotedUsersIds.Any(id => id == userId);
    }
    private async Task<bool> CheckUserHasVotedQuestion(string userId, int postId)
    {
        var post = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id ==  postId)
            ?? throw new EntityNotFoundException();

        return post.VotedUsersIds != null && post.VotedUsersIds.Any(id => id == userId);
    }
}
