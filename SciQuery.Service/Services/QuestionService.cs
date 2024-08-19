﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;
using System.Reflection.Metadata.Ecma335;

namespace SciQuery.Service.Services;

public class QuestionService(SciQueryDbContext dbContext,
    IMapper mapper,
    IFileManagingService fileManaging,
    ICommentService commentService)
    : IQuestionService
{
    private readonly SciQueryDbContext _context = dbContext
        ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IFileManagingService _fileManaging = fileManaging
       ?? throw new ArgumentNullException(nameof(mapper));

    private readonly ICommentService _commentService  = commentService;


    public async Task<PaginatedList<ForEasyQestionDto>> GetQuestionsByTags(int id)
    {
        var tags = await _context.Questions
                .Where(x => x.Id == id)
                .SelectMany(x => x.QuestionTags.Select(qt => qt.Tag))
                .ToListAsync() ?? throw new EntityNotFoundException();

        if (tags == null || tags.Count == 0)
        {
            throw new Exception();
        }

        var result = _context.Tags
            .Where(x => tags.Contains(x))
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
            .ToPaginatedList<ForEasyQestionDto, Question>(_mapper.ConfigurationProvider);

        return questions;
    }
    public async Task<PaginatedList<ForEasyQestionDto>> GetAllAsync(QuestionQueryParameters queryParams)
    {
        var query = _context.Questions
            .AsQueryable()
            .Include(q => q.User)
            .Include(q =>    q.Answers)
            .Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .AsNoTracking();


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

        var result = await query
            .ToPaginatedList<ForEasyQestionDto, Question>(_mapper.ConfigurationProvider, queryParams.PageNumber, queryParams.PageSize);
        
        var questionIds = await query
            .Select(q => q.Id)
            .ToListAsync();

        var comments = await _context.Comments
            .Where(c => questionIds.Contains(c.PostId) && c.Post == PostType.Question)
            .GroupBy(c => c.PostId)
            .Select(g => new
            {
                PostId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        foreach (var question in result.Data)
        {
            var comment = comments.FirstOrDefault(c => c.PostId == question.Id);
            question.CommentsCount = comment != null ? comment.Count : 0;
        }

        return result;
    }
    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .Where(q => q.Id == id)
            .Include(q => q.User)
            .Include(q => q.Answers)
            .Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id)
            ?? throw new EntityNotFoundException($"Question does not found {id}");

        question.Comments = await _context.Comments
            .Where(c => c.Post == PostType.Question && c.PostId == id)
            .Include(c => c.User)
            .AsNoTracking()
            .ToListAsync();
        
        var dto = _mapper.Map<QuestionDto>(question);

        if(question.ImagePaths == null || question.ImagePaths.Count < 1)
        {
            return dto;
        }

        foreach (var imagePath in question.ImagePaths)
        {
            
            var image = await fileManaging.DownloadFileAsync(imagePath, "QuestionImages");
            dto.Images!.Add(image);
        }

        return dto
            ?? throw new EntityNotFoundException($"Question with id : {id} is not found!");
    }

    public async Task<QuestionDto> CreateAsync(QuestionForCreateDto questionCreateDto)
    {
        
        // Question ob'ektini yaratish va maplash
        var question = _mapper.Map<Question>(questionCreateDto);
        question.CreatedDate = DateTime.Now;
        question.UpdatedDate = DateTime.Now;

        // Question ob'ektini saqlash
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        // Teglar va ularning bog'lanishlarini qo'shish
        var tags = new List<Tag>();
        foreach (var tagName in questionCreateDto.Tags)
        {
            var tag = await _context.Tags.SingleOrDefaultAsync(t => t.Name == tagName);

            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                tags.Add(tag);
            }
            else
            {
                tags.Add(tag);
            }
        }

        // Teglarni birgalikda qo'shish
        if (tags.Any(t => t.Id == 0)) // Yangi taglar mavjud bo'lsa
        {
            _context.Tags.AddRange(tags.Where(t => t.Id == 0));
            await _context.SaveChangesAsync();
        }

        // QuestionTag bog'lanishlarini yaratish
        var questionTags = tags.Select(t => new QuestionTag
        {
            QuestionId = question.Id,
            TagId = t.Id
        }).ToList();

        _context.QuestionTags.AddRange(questionTags);
        await _context.SaveChangesAsync();

        // Yangi yaratilgan QuestionDto qaytarish
        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<string> CreateImages(IFormFile file)
    {
        return await _fileManaging.UploadFile(file, "Source", "Images", "QuestionImages");
    }

    public async Task UpdateAsync(int id, QuestionForUpdateDto questionUpdateDto)
    {
        var question = await _context.Questions.FindAsync(id) 
            ?? throw new EntityNotFoundException($"Question with id : {id} is not found!");
        
        question.UpdatedDate = DateTime.Now;

        _mapper.Map(questionUpdateDto, question);
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
        {
            return false;
        }

        await _commentService.DeleteCommentByPostIdAsync(PostType.Question, question.Id);   

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        return true;
    }
}

