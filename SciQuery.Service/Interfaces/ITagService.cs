using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Tag;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Interfaces;

public interface ITagService
{
    Task<PaginatedList<TagDto>> GetAllTagsAsync(TagQueryParameters queryParameters);
    Task<TagDto> GetTagByIdAsync(int id);
    Task<TagDto> CreateTagAsync(TagForCreateAndUpdateDto tag);
    Task<TagDto> UpdateTagAsync(int id, TagForCreateAndUpdateDto tag);
    Task<bool> DeleteTagAsync(int id);
}
