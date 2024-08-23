using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Interfaces;

public interface ICommentService
{
    Task<CommentDto> GetCommentByIdAsync(int id);
    Task<PaginatedList<CommentDto>> GetAllComments(CommentQueryParameters queryParameters);
    Task<CommentDto> CreateCommentAsync(CommentForCreateDto commentCreateDto);
    Task<CommentDto> UpdateCommentAsync(int id, CommentForUpdateDto commentUpdateDto);
    Task<bool> DeleteCommentAsync(int id);
    Task DeleteCommentByPostIdAsync(PostType postType,int postId);
}
