using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.Interfaces;
using AutoMapper.QueryableExtensions;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.Mappings.Extensions;

namespace SciQuery.Service.Services;

public class AnswerService(SciQueryDbContext context, IMapper mapper, IFileManagingService fileManaging, ICommentService commentService) : IAnswerService
{
    private readonly SciQueryDbContext _context = context
        ??throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IFileManagingService _fileManaging = fileManaging
       ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ICommentService _commentService = commentService;

    public async Task<AnswerDto> GetByIdAsync(int id)
    {
        var answer = await _context.Answers
            .AsQueryable()
            .Include(a => a.User)
            .Include(a => a.Question)
            .Include(a => a.Comments)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");

        answer.Comments = await _context.Comments
            .Where(c => c.Post == PostType.Question && c.PostId == id)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<AnswerDto>(answer);
    }

    public async Task<PaginatedList<AnswerDto>> GetAllAnswersByQuestionIdAsync(int questionId)
    {
        // Step 1: Fetch all answers for the given question
        var answers = await _context.Answers
            .Include(a => a.User) // Include the user who posted the answer
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.Id)
            .AsNoTracking()
            .ToPaginatedList<AnswerDto, Answer>(_mapper.ConfigurationProvider, 1, 15);

        // Step 2: Fetch comments for all answers
        var answerIds = answers.Data
            .Select(a => a.Id)
            .ToList();

        var comments = await _context.Comments
            .Include(c => c.User) // Include the user who posted the comment
            .Where(c => answerIds.Contains(c.PostId) && c.Post == PostType.Answer)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        // Step 3: Associate comments with their respective answers
        foreach (var answer in answers.Data)
        {
            answer.Comments = comments
                .Where(c => c.PostId == answer.Id)
                .ToList();
        }

        return answers;
    }

    public async Task<AnswerDto> CreateAsync(AnswerForCreateDto answerCreateDto)
    {
        var answer = _mapper.Map<Answer>(answerCreateDto);
        answer.CreatedDate = DateTime.Now;
        answer.UpdatedDate = DateTime.Now;

        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnswerDto>(answer);
    }

    public async Task<List<string>> CreateImages(List<IFormFile> files)
    {
        return await _fileManaging.UploadAnswersImagesAsync(files);
    }

    public async Task UpdateAsync(int id, AnswerForUpdateDto answerUpdateDto)
    {
        var answer = await _context.Answers.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");
        answer.UpdatedDate = DateTime.Now;

        _mapper.Map(answerUpdateDto, answer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");

        await _commentService.DeleteCommentByPostIdAsync(PostType.Answer, id);

        _context.Answers.Remove(answer);    

        await _context.SaveChangesAsync();
    }

}
