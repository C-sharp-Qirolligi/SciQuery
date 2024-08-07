﻿using Microsoft.AspNetCore.Http;
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
    public async Task<string> UploadFileAsync(IFormFile file)
    {

        if (file is null)
        {
            throw new ArgumentNullException("File cannot be null.");
        }

        if (file.Length == 0 || file.Length > 1024 * 1024 * 40)
        {
            throw new ArgumentNullException("File must be image!");
        }

        var fileName = Guid.NewGuid() + file.FileName;

        var path = Directory.GetCurrentDirectory();

        /*if (file.ContentType == "image/jpeg" || file.ContentType == "image/png")
        {
            path = Path.Combine(
                "uploads",
                "images",
                fileName);
        }
        else
        {
            path = Path.Combine(
                "uploads",
                "Files",
                fileName);
        }*/
        path = Path.Combine(path,"source","Images","userImages",fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
    public async Task<UserFiles> DownloadFileAsync(string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Source ");

        if (File.Exists(Path.Combine(path, "Images/UserImages", fileName)))
        {
            path = Path.Combine(path, "images/UserImage", fileName);
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

        return new(bytes, contentType, Path.GetFileName(path));
    }

    /*Task<string> IFileManagingService.UploadFileAsync(IFormFile file)
    {
        throw new NotImplementedException();
    }*/

    /*Task<UserFiles> IFileManagingService.DownloadFileAsync(string path)
    {
        throw new NotImplementedException();
    }*/


}
