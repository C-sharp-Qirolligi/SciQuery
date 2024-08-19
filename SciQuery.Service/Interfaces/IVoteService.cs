using SciQuery.Domain.Entities;

namespace SciQuery.Service.Interfaces;

public interface IVoteService
{
    Task UpVote(string userId,int postId,PostType postType);
    Task DownVote(string userId,int postId,PostType postType);
}
