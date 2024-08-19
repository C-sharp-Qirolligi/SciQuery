using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Interfaces;

public interface IAnswerService
{
    Task<PaginatedList<AnswerDto>> GetAllAnswersByQuestionIdAsync(int questionId, AnswerQueryParameters answerQueryParameters);
    Task<AnswerDto> GetByIdAsync(int id);
    Task<AnswerDto> CreateAsync(AnswerForCreateDto answer);
    Task<string> CreateImages(IFormFile file);
    Task UpdateAsync(int id, AnswerForUpdateDto answer);
    Task DeleteAsync(int id);
}
