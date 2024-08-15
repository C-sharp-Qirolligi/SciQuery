using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Comment;

namespace SciQuery.Service.Interfaces;

public interface ICommentService
{
    Task<CommentDto> GetCommentByIdAsync(int id);
    Task<CommentDto> GetCommentByUserIdAsync(int id);
    Task<IEnumerable<CommentDto>> GetAllCommentsByQuestionIdAsync(int questionId);
    Task<IEnumerable<CommentDto>> GetAllCommentsByAnswerIdAsync(int answerId);
    Task<CommentDto> CreateCommentAsync(CommentForCreateDto commentCreateDto);
    Task<CommentDto> UpdateCommentAsync(int id, CommentForUpdateDto commentUpdateDto);
    Task<bool> DeleteCommentAsync(int id);
    Task DeleteCommentByPostIdAsync(PostType postType,int postId);
}
