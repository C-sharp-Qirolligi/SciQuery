using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.Interfaces;

public interface IFileManagingService
{
    Task<string> UploadFile(IFormFile file,params string[] imagePath);

    Task<UserFiles> DownloadFileAsync(string path);
}
