using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.Interfaces;

namespace SciQuery.Service.Services;

public class CommentService(SciQueryDbContext context, IMapper mapper) : ICommentService
{
    private readonly SciQueryDbContext _context = context;
    private readonly IMapper _mapper = mapper;

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

    public async Task<IEnumerable<CommentDto>> GetAllCommentsByQuestionIdAsync(int questionId)
    {
        var comments = await _context.Comments
            .Where(c => c.QuestionId == questionId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
    public Task<CommentDto> GetCommentByUserIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CommentDto>> GetAllCommentsByAnswerIdAsync(int answerId)
    {
        var comments = await _context.Comments
            .Where(c => c.AnswerId == answerId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto> CreateCommentAsync(CommentForCreateDto commentCreateDto)
    {
        var comment = _mapper.Map<Comment>(commentCreateDto);

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> UpdateCommentAsync(int id, CommentForUpdateDto commentUpdateDto)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return null;
        }

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
}
