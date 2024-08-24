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
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Services;

public class AnswerService(SciQueryDbContext context,
                            IMapper mapper, 
                            IFileManagingService fileManaging,
                            ICommentService commentService,
                            NotificationService notificationService) : IAnswerService
{
    private readonly SciQueryDbContext _context = context
        ??throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IFileManagingService _fileManaging = fileManaging
       ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ICommentService _commentService = commentService;
    private readonly NotificationService _notificationService = notificationService;

    public async Task<AnswerDto> GetByIdAsync(int id)
    {
        var answer = await _context.Answers
            .AsQueryable()
            .Include(a => a.User)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new EntityNotFoundException($"Answer with id : {id} is not found!");

        var dto = _mapper.Map<AnswerDto>(answer);
        
        dto.User.Image = await fileManaging.DownloadFileAsync(answer.User.ImagePath!,"UserImages");
        
        await FetchImagesForAnswersAsync(dto);

        return dto;
    }
    public async Task<PaginatedList<AnswerDto>> GetAllAnswersByQuestionIdAsync(int questionId, AnswerQueryParameters answerQueryParameters)
    {

        // Fetch paginated answers
        var answers = await _context.Answers
            .Include(a => a.User)
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.Id)
            .AsNoTracking()
            .ToPaginatedList<AnswerDto, Answer>(_mapper.ConfigurationProvider, answerQueryParameters.PageNumber, answerQueryParameters.PageSize);

        var answerIds = answers.Data.Select(a => a.Id).ToList();

        // Fetch images in a separate method
        foreach(var answer in answers.Data)
        {
            await FetchImagesForAnswersAsync(answer);
            answer.User.Image = await fileManaging.DownloadFileAsync(answer.User.ImagePath!, "UserImages");
        }
        
        return answers;
    }

    private async Task FetchImagesForAnswersAsync(AnswerDto answer)
    {
        if(answer is null || answer.ImagePaths is null)
        {
            return;
        }
        var images = new List<ImageFile>();

        foreach (var imagePath in answer.ImagePaths ?? Enumerable.Empty<string>())
        {
            var image = await fileManaging.DownloadFileAsync(imagePath, "AnswerImages");
            images.Add(image);
        }

        answer.Images = images;
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
            var message = $"{answer.User.UserName} tomonidan savolingiga javob berildi";
            await _notificationService.NotifyUser(question.UserId, message);
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
