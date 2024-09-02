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
                            INotificationService notificationService) : IAnswerService
{
    private readonly SciQueryDbContext _context = context
        ??throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IFileManagingService _fileManaging = fileManaging
       ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ICommentService _commentService = commentService;
    private readonly INotificationService _notificationService = notificationService;

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
    public async Task<PaginatedList<AnswerDto>> GetAll(AnswerQueryParameters answerQueryParameters)
    {

        // Fetch paginated answers
        var query = _context.Answers
            .Include(a => a.User).AsQueryable();

        if(answerQueryParameters.UserId != null)
        {
            query = query.Where(x => x.UserId == answerQueryParameters.UserId); 
        }
        if(answerQueryParameters.QuestionId != null)
        {
            query = query.Where(x => x.QuestionId == answerQueryParameters.QuestionId);
        }

        var answers = await query
            .OrderByDescending(a => a.CreatedDate)
            .ThenByDescending(x => x.UpdatedDate)
            .AsNoTracking()
            .ToPaginatedList<AnswerDto, Answer>(_mapper.ConfigurationProvider, answerQueryParameters.PageNumber, answerQueryParameters.PageSize);

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

        var createdAnswer = _context.Answers.Add(answer).Entity;
        await _context.SaveChangesAsync();

        // Savolni yaratgan foydalanuvchiga bildirishnoma yuborish
        var question = await _context.Questions
            .Include(q => q.User)
            .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);


        var notification = new Notification()
        {
            QuestionId = question.Id,
            Message = $"💡 {answer.User.UserName} tomonidan savolingiga javob berildi",
            TimeSpan = DateTime.Now,
            IsRead = false,
            UserId = question.UserId,
        };

        await _notificationService.NotifyUser(notification);
        await _notificationService.AddNotification(notification);
            
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

        // Ensure the ImagePaths in the questionUpdateDto is not null
        var updatedImagePaths = answerUpdateDto.ImagePaths ?? new List<string>();

        // Get the images that need to be deleted
        var imagesToDelete = answer.ImagePaths
            .Except(updatedImagePaths)
            .ToList();

        // Delete each image that is not in the updated list
        foreach (var imagePath in imagesToDelete)
        {
            _fileManaging.DeleteFile(imagePath, "AnswerImages");
        }

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
