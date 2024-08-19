using SciQuery.Domain.Entities;

namespace SciQuery.Service.Interfaces;

public interface IVoteService
{
    Task<(bool, string)> UpVote(string userId,int postId,PostType postType);
    Task<(bool, string)> DownVote(string userId,int postId,PostType postType);
}
