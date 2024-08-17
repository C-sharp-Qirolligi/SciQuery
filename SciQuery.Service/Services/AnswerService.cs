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
using Microsoft.AspNetCore.SignalR;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.Services;

public class AnswerService(SciQueryDbContext context,
                            IMapper mapper, 
                            IFileManagingService fileManaging,
                            ICommentService commentService,
                            IHubContext<NotificationHub> hubContext) : IAnswerService
{
    private readonly SciQueryDbContext _context = context
        ??throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IFileManagingService _fileManaging = fileManaging
       ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ICommentService _commentService = commentService;
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;

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

        var dto = _mapper.Map<AnswerDto>(answer);

        var images = new List<ImageFile>();

        foreach (var imagePath in answer.ImagePaths)
        {
            var image = await fileManaging.DownloadFileAsync(imagePath, "AnswerImages");
            images.Add(image);
        }
        dto.Images = images;

        return dto;
    }

    public async Task<PaginatedList<AnswerDto>> GetAllAnswersByQuestionIdAsync(int questionId, int pageNumber, int pageSize)
    {
        // Step 1: Fetch all answers for the given question with pagination
        var answers = await _context.Answers
            .Include(a => a.User) // Include the user who posted the answer
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.Id)
            .AsNoTracking()
            .ToPaginatedList<AnswerDto, Answer>(_mapper.ConfigurationProvider, pageNumber, pageSize);

        // Step 2: Fetch comments for all answers
        var answerIds = answers.Data
            .Select(a => a.Id)
            .ToList();

        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => answerIds.Contains(c.PostId) && c.Post == PostType.Answer)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var answerImageTasks = answers.Data.Select(async answerDto =>
        {
            var answer = await _context.Answers
                .Where(a => a.Id == answerDto.Id)
                .Select(a => new { a.ImagePaths })
                .FirstOrDefaultAsync();

            var images = new List<ImageFile>();

            foreach (var imagePath in answer?.ImagePaths ?? Enumerable.Empty<string>())
            {
                var image = await fileManaging.DownloadFileAsync(imagePath, "AnswerImages");
                images.Add(image);
            }

            answerDto.Images = images;
        });

        // Await all image fetching tasks
        await Task.WhenAll(answerImageTasks);

        // Step 4: Associate comments with their respective answers
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

        // Savolni yaratgan foydalanuvchiga bildirishnoma yuborish
        var question = await _context.Questions
            .Include(q => q.User)
            .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

        if (question != null)
        {
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", "Sizning savolingizga yangi javob qo'shildi!");
        }

        return _mapper.Map<AnswerDto>(answer);
    }


    public async Task<string> CreateImages(IFormFile file)
    {
        return await _fileManaging.UploadFile(file,"Source","Images","AnswerImages");
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
