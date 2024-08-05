using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.DTOs.Tag;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Services;

public class QuestionService(SciQueryDbContext dbContext,IMapper mapper) : IQuestionService
{
    private readonly SciQueryDbContext _context = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedList<ForEasyQestionDto>> GetQuestionsByTags(QuestionQueryParameters queryParams)
    {
        if(queryParams.Tags == null || queryParams.Tags.Count == 0)
        {
            throw new Exception();
        }

        var result = _context.Tags
            .Where(x => queryParams.Tags.Contains(x.Name))
            .Join(_context.QuestionTags,
                t => t.Id,
                qt => qt.TagId,
                (t, qt) => new { qt.QuestionId })
            .GroupBy(q => q.QuestionId)
            .OrderByDescending(g => g.Count())
            .Select(g => new { QuestionId = g.Key, Count = g.Count() });

        var questions = await result
            .Join(_context.Questions,
                  r => r.QuestionId,
                  q => q.Id,
                  (r, q) => q)
            .AsNoTracking()
            .ToPaginatedList<ForEasyQestionDto, Question>(_mapper.ConfigurationProvider,1,15);

        return questions;
    }
    public async Task<PaginatedList<ForEasyQestionDto>> GetAllAsync(QuestionQueryParameters queryParams)
    {
        var query = _context.Questions
        .Include(q => q.User)
        .Include(q => q.Answers)
        .Include(q => q.Votes)
        .Include(q => q.QuestionTags)
        .ThenInclude(qt => qt.Tag)
        .AsNoTracking()
        .AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            query = query.Where(q => q.Title.Contains(queryParams.Search) 
                    || q.Body.Contains(queryParams.Search));
        }

        if (queryParams.LastDate.HasValue)
        {
            query = query.Where(q => q.CreatedDate <= queryParams.LastDate.Value);
        }

        if (queryParams.AnswerMaxCount.HasValue)
        {
            query = query.Where(q => q.Answers.Count <= queryParams.AnswerMaxCount.Value);
        }

        if (queryParams.AnswerMinCount.HasValue)
        {
            query = query.Where(q => q.Answers.Count >= queryParams.AnswerMinCount.Value);
        }

        if (queryParams.NewAsc.HasValue && queryParams.NewAsc == true)
        {
            query = query.OrderByDescending(x => x.UpdatedDate);
        }
        else
        {
            query = query.OrderBy(x => x.UpdatedDate);
        }

        var result =  await query.ToPaginatedList<ForEasyQestionDto, Question>(_mapper.ConfigurationProvider, 1, 15);
        return result;
    }
    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.User)
            .Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .Include(q => q.Comments)
            .Include(q => q.Votes)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(q => q.Id == id);

        var dto = _mapper.Map<QuestionDto>(question);

        return dto
            ?? throw new EntityNotFoundException($"Question with id : {id} is not found!");
    }

    public async Task<QuestionDto> CreateAsync(QuestionForCreateDto questionCreateDto)
    {
        var question = _mapper.Map<Question>(questionCreateDto);
        question.CreatedDate = DateTime.Now;
        question.UpdatedDate = DateTime.Now;

        var created = _context.Questions.Add(question).Entity;
        await _context.SaveChangesAsync();

        foreach (var tagName in questionCreateDto.Tags)
        {
            var tag = await _context.Tags.SingleOrDefaultAsync(t => t.Name == tagName);
            
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            var questionTag = new QuestionTag
            {
                QuestionId = question.Id,
                TagId = tag.Id
            };

            _context.QuestionTags.Add(questionTag);
        }

        return _mapper.Map<QuestionDto>(created);
    }

    public async Task UpdateAsync(int id, QuestionForUpdateDto questionUpdateDto)
    {
        var question = await _context.Questions.FindAsync(id) 
            ?? throw new EntityNotFoundException($"Question with id : {id} is not found!");
        
        question.UpdatedDate = DateTime.Now;

        _mapper.Map(questionUpdateDto, question);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id)
            ?? throw new EntityNotFoundException($"Question with id : {id} is not found!");


        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
    }
}

