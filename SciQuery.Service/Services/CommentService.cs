using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Services;

public class CommentService(SciQueryDbContext context,
    IMapper mapper,
    IFileManagingService fileMangingService,
    NotificationService notificationService
    ) : ICommentService
{
    private readonly SciQueryDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IFileManagingService _fileMangingService = fileMangingService;
    private readonly NotificationService _notificationService = notificationService;

    public async Task<CommentDto> GetCommentByIdAsync(int id)
    {
        var comment = await _context.Comments
            .Include(a => a.User)
            .AsNoTracking()
            .AsSplitQuery() 
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new EntityNotFoundException($"Comment with id : {id} is not found!");
        if (comment == null)
        {
            return null;
        }
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<PaginatedList<CommentDto>> GetAllComments(CommentQueryParameters queryParameters)
    {
        var query = _context.Comments
            .AsQueryable();
        if (queryParameters.PostType.HasValue && queryParameters.PostType == (int)PostType.Answer && queryParameters.PostId.HasValue)
        {
            query = query.Where(c => c.Post == PostType.Answer && c.PostId == queryParameters.PostId);
        }
        if (queryParameters.PostType.HasValue && queryParameters.PostType == (int)PostType.Question && queryParameters.PostId.HasValue)
        {
            query = query.Where(c => c.Post == PostType.Question && c.PostId == queryParameters.PostId);
        }
        if (!string.IsNullOrWhiteSpace(queryParameters.UserId))
        {
            query = query.Where(c => c.UserId == queryParameters.UserId);
        }
        var comments = await query
            .ToPaginatedList<CommentDto, Comment>(_mapper.ConfigurationProvider, queryParameters.PageNumber, queryParameters.PageSize);

        foreach (var comment in comments.Data)
        {
            comment.User.Image = await _fileMangingService.DownloadFileAsync(comment.User.ImagePath, "UserImages");
        }
        return comments;
    }

    public async Task<CommentDto> CreateCommentAsync(CommentForCreateDto commentCreateDto)
    {
        var comment = _mapper.Map<Comment>(commentCreateDto);

        var createdComment = _context.Comments.Add(comment).Entity;
        await _context.SaveChangesAsync();

        var user = createdComment.User;
        if (commentCreateDto.Post == PostType.Question)
        {
            user = await _context.Questions
                .Include(c => c.User)
                .Where(c => c.Id == commentCreateDto.PostId)
                .Select(c => c.User)
                .FirstOrDefaultAsync();
        }
        else
        {
            user = await _context.Answers
                .Include(c => c.User)
                .Where(c => c.Id == commentCreateDto.PostId)
                .Select(c => c.User)
                .FirstOrDefaultAsync();
        }

        var postType = commentCreateDto.Post == PostType.Answer ? "javob" : "savol";
        
        await _notificationService.NotifyUser(user.Id, $"Sizning {postType}ingizga {user.UserName} tomonidan fikr yozildi");

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> UpdateCommentAsync(int id, CommentForUpdateDto commentUpdateDto)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id) 
            ?? throw new EntityNotFoundException("Comment not Found");
        
        _mapper.Map(commentUpdateDto, comment);
        await _context.SaveChangesAsync();

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return false;
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task DeleteCommentByPostIdAsync(PostType postType,int postId)
    {
        var comments = await _context.Comments
            .Where(c => c.Post == postType && c.PostId == postId)
            .ToListAsync();

        _context.Comments.RemoveRange(comments);
        _context.SaveChanges();
        
    }
}
