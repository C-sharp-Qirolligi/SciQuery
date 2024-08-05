using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Tag;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Interfaces;

public interface ITagService
{
    Task<PaginatedList<TagDto>> GetAllTagsAsync(TagQueryParameters queryParameters);
    Task<Tag> GetTagByIdAsync(int id);
    Task<TagDto> CreateTagAsync(TagForCreateDto tag);
    Task<Tag> UpdateTagAsync(int id, Tag tag);
    Task<bool> DeleteTagAsync(int id);
}
