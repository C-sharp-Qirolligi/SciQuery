using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.Interfaces;

public interface IFileManagingService
{
    Task<string> UploadUserImagesAsync(IFormFile file);

    Task<List<string>> UploadQuestionImagesAsync(List<IFormFile> files);

    Task<List<string>> UploadAnswersImagesAsync(List<IFormFile> files);

    Task<UserFiles> DownloadFileAsync(string path);
}
