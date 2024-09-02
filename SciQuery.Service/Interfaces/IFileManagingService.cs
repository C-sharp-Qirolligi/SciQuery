using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.Interfaces;

public interface IFileManagingService
{
    Task<string> UploadFile(IFormFile file,params string[] imagePath);

    Task<ImageFile> DownloadFileAsync(string fileName,string filePath);
    void DeleteFile(string fileName, string filePath);
}
