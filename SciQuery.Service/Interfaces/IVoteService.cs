using SciQuery.Service.DTOs.Vote;
using SciQuery.Service.Pagination.PaginatedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.Interfaces
{
    public interface IVoteService
    {
        Task<PaginatedList<VoteDto>> GetVoteByUserIdAsync(string userId);
        Task<PaginatedList<VoteDto>> GetAllVotesByQuestionIdAsync(int questionId);
        Task<PaginatedList<VoteDto>> GetAllVotesByAnswerIdAsync(int answerId);
        Task<VoteDto> CreateVoteAsync(VoteForCreateDto voteCreateDto);
        Task<bool> DeleteVoteAsync(int id);
    }
}
