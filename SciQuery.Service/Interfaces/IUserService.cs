﻿using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.User;
using SciQuery.Service.Pagination.PaginatedList;

namespace SciQuery.Service.Interfaces;
public interface IUserService
{
    Task<PaginatedList<UserDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> CreateAsync(UserForCreateDto userCreateDto);

    Task<string> CreateImage(IFormFile file);
   
    Task UpdateAsync(string id, UserForUpdatesDto userUpdateDto);
    Task<bool> DeleteAsync(string id);

}
