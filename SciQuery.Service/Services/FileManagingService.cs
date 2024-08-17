using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using SciQuery.Service.DTOs.User;
using SciQuery.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.Services;

public class FileMangingService(FileExtensionContentTypeProvider fileExtension) : IFileManagingService
{
    private readonly FileExtensionContentTypeProvider _fileExtension = fileExtension;
  
    public async Task<string> UploadFile(IFormFile file,params string[] imagePath)
    {
        if (file == null)
        {
            throw new ArgumentNullException("Files cannot be null or empty.");
        }

        if (file.Length == 0 || file.Length > 1024 * 1024 * 40)
        {
            throw new ArgumentException("Each file must be an image with a size up to 40 MB.");
        }

        var fileName = Guid.NewGuid() + file.FileName;

        var path = Directory.GetCurrentDirectory();
        
        foreach(var directory in imagePath)
        {
            path = Path.Combine(path, directory);
        }

        path  = Path.Combine(path, fileName);
        
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    public async Task<UserFiles> DownloadFileAsync(string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Source");

        if (File.Exists(Path.Combine(path, "Images/QuestionImages", fileName)))
        {
            path = Path.Combine(path, "images/questionImages", fileName);
        }
        else
        {
            throw new FileNotFoundException("File not found.");
        }

        if (!_fileExtension.TryGetContentType(path, out string contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(path);

        var image = new UserFiles(bytes, contentType, Path.GetFileName(path));
        return image;
    }
}
