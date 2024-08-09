using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.Pagination.PaginatedList;

namespace SciQuery.Service.Interfaces;

public interface IAnswerService
{
    Task<PaginatedList<AnswerDto>> GetAllAnswersByQuestionIdAsync(int questionId);
    Task<AnswerDto> GetByIdAsync(int id);
    Task<AnswerDto> CreateAsync(AnswerForCreateDto answer);
    Task<List<string>> CreateImages(List<IFormFile> files);
    Task UpdateAsync(int id, AnswerForUpdateDto answer);
    Task DeleteAsync(int id);
}
