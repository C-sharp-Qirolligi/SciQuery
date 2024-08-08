using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;

namespace SciQuery.Service.Services;

public class AnswerService(SciQueryDbContext context, IMapper mapper, IFileManagingService fileManaging) : IAnswerService
{
    private readonly SciQueryDbContext _context = context
        ??throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IFileManagingService _fileManaging = fileManaging
       ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<AnswerDto> GetByIdAsync(int id)
    {
        var answer = await _context.Answers
            .Include(a => a.User)
            .Include(a => a.Question)
            .Include(a => a.Comments)
            .Include(a => a.Votes)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");

        return _mapper.Map<AnswerDto>(answer);
    }

    public async Task<PaginatedList<AnswerDto>> GetAllAnswersByQuestionIdAsync(int questionId)
    {
        var answers = await _context.Answers
            .Include(a => a.User)
            .Include(a => a.Question)
            .Include(a => a.Votes)
            .Where(a => a.QuestionId == questionId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToPaginatedList<AnswerDto, Answer>(_mapper.ConfigurationProvider, 1, 15);

        return answers;
    }

    public async Task<AnswerDto> CreateAsync(AnswerForCreateDto answerCreateDto)
    {
        var answer = _mapper.Map<Answer>(answerCreateDto);
        answer.CreatedDate = DateTime.Now;

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
        var answer = await _context.Answers.FindAsync(id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");
        answer.UpdatedDate = DateTime.Now;

        _mapper.Map(answerUpdateDto, answer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _context.Answers
            .Include(a => a.Comments)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");
        
        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();
    }

}
