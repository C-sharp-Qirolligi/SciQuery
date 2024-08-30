using AutoMapper;
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
using System.Xml;

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
                .SelectMany(x => x.QuestionTags.Select(qt => qt.Tag.Name))
                .ToListAsync() ?? throw new EntityNotFoundException();

        var query = _context.Questions
            .AsNoTracking()
            .AsQueryable();
        
        query = GetQuestionsByTags(tags, query);

        var questions = await query.ToPaginatedList<ForEasyQestionDto, Question>(_mapper.ConfigurationProvider);
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

        if (!string.IsNullOrEmpty(queryParams.UserId))
        {
            query = query.Where(q => q.UserId == queryParams.UserId);
        }
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            query = query.Where(q => q.Title.Contains(queryParams.Search)
                    || q.Body.Contains(queryParams.Search));
        }

        if (queryParams.NoAnswers.HasValue && queryParams.NoAnswers == true)
        {
            query = query.Where(q => q.Answers == null || q.Answers.Count <= 0);
        }

        if (queryParams.NoAcceptedAnswer.HasValue && queryParams.NoAcceptedAnswer == true)
        {
            query = query.Where(q => q.AcceptedAnswers == null || q.AcceptedAnswers.Count==0);
        }
        
        if(queryParams.Tags != null && queryParams.Tags.Count > 0 && queryParams.Tags.All(tag => !string.IsNullOrEmpty(tag)))
        {
            query = GetQuestionsByTags(queryParams.Tags,query);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.SortBy) && queryParams.SortBy == QuerySortingParametersConstants.MostVoted)
        {
            query = query.OrderByDescending(x => x.Votes);
        }
        
        else
        {
            query = query.OrderByDescending(x => x.UpdatedDate).ThenByDescending(x => x.CreatedDate);
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

        //Bu yerini to'girlash kerak . Commentlar lazy loading asosida yuklansin yoki umuman yuklanmasin
        foreach (var question in result.Data)
        {
            var comment = comments.FirstOrDefault(c => c.PostId == question.Id);
            question.CommentsCount = comment != null ? comment.Count : 0;
            question.User.Image = await fileManaging.DownloadFileAsync(question.User.ImagePath, "UserImages");
        }
        return result;
    }
    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .Where(q => q.Id == id)
            .Include(q => q.User)
            .Include(q => q.QuestionTags)
            .ThenInclude(qt => qt.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id)
            ?? throw new EntityNotFoundException($"Question does not found {id}");
        
        var dto = _mapper.Map<QuestionDto>(question);

        dto.User.Image = await fileManaging.DownloadFileAsync(dto.User.ImagePath!, "UserImages");

        foreach (var imagePath in question.ImagePaths ?? new() )
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
        await CreateAndConnectTags(question, questionCreateDto.Tags);

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
        var question = await _context.Questions
            .Include(q => q.QuestionTags)
            .ThenInclude(a => a.Tag)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new EntityNotFoundException($"Question with id : {id} is not found!");
        
        question.UpdatedDate = DateTime.Now;

        //If question have a any question tags ,remove they. In end of method clear tags which have not any question tags!
        _context.QuestionTags.RemoveRange(question.QuestionTags.Where(x => !questionUpdateDto.Tags.Contains(x.Tag.Name)));
        _context.SaveChanges();

        //kelgan questionni yangi teglarga bog'lash yoki yangi teglar yaratish
        var oldTags = question.QuestionTags.Select(qt => qt.Tag.Name).ToList();
        var newTags = questionUpdateDto.Tags.Where(c => !oldTags.Contains(c)).ToList();

        await CreateAndConnectTags(question, newTags);

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
    //Taglarni yaratish va savolga bo'g'lash mantig'ini chiqarib olish
    private async Task CreateAndConnectTags(Question question,List<string>newTags)
    {
        //change all incoming tags toLower case
        newTags = newTags.Select(x => x.ToLower()).ToList();

        //to track and create new tags
        var tags = new List<Tag>();

        //If tag is exist connect to question it by createing questionTags. If Does not exist create tag
        foreach (var tagName in newTags)
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

        //Bo'sh bo'lib qogan teglarni tozalash
        await ClearEmptyTags();
    }
    private async Task ClearEmptyTags()
    {
        var tags = await _context.Tags.Where(t => t.QuestionTags.Count == 0).ToListAsync();
        _context.RemoveRange(tags);
    }
    private IQueryable<Question> GetQuestionsByTags(ICollection<string>tags,IQueryable<Question>query)
    {
        if (tags == null || tags.Count == 0)
        {
            return query;
        }

        var result = _context.Tags
            .Where(x => tags.Contains(x.Name))
            .Join(_context.QuestionTags,
                t => t.Id,
                qt => qt.TagId,
                (t, qt) => new { qt.QuestionId })
            .GroupBy(q => q.QuestionId)
            .OrderByDescending(g => g.Count())
            .Select(g => new { QuestionId = g.Key, Count = g.Count() });

        var questions = result
            .Join(_context.Questions,
                  r => r.QuestionId,
                  q => q.Id,
                  (r, q) => q)
            .AsNoTracking();
        
        return questions;
    }
}

