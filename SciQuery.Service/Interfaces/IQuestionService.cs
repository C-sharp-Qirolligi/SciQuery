using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.DTOs.Tag;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Interfaces;

public interface IQuestionService
{
    Task<PaginatedList<ForEasyQestionDto>> GetQuestionsByTags(QuestionQueryParameters queryParams);
    Task<PaginatedList<ForEasyQestionDto>> GetAllAsync(QuestionQueryParameters queryParams);
    Task<QuestionDto> GetByIdAsync(int id);
    Task<QuestionDto> CreateAsync(QuestionForCreateDto question);   
    Task UpdateAsync(int id,QuestionForUpdateDto question);
    Task DeleteAsync(int id);
}
