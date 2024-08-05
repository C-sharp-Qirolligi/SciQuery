using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.User;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;

namespace SciQuery.Service.Services;

public class UserService(UserManager<User> user,IMapper mapper) : IUserService
{
    private readonly UserManager<User> _userManager = user ?? throw new ArgumentNullException(nameof(user));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    public async Task<PaginatedList<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToPaginatedList<UserDto, User>(_mapper.ConfigurationProvider,1,15);
        return users;
    }
    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await _userManager
            .FindByIdAsync(id) 
            ?? throw new EntityNotFoundException($"User with id : {id} is not found!");
        UserDto userDto = _mapper.Map<UserDto>(user);

        return userDto;
    }
    public async Task<UserDto> CreateAsync(UserForCreateDto userCreateDto)
    {
        var user = _mapper.Map<User>(userCreateDto);
        user.CreatedDate = DateTime.UtcNow;

        var result = await _userManager.CreateAsync(user, userCreateDto.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Something wrong with craeting user");
        }

        return _mapper.Map<UserDto>(user);
    }
    public async Task UpdateAsync(int id, UserForUpdatesDto userUpdateDto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString())
            ?? throw new EntityNotFoundException($"User with id : {id} is not found!");
        user.UserName = userUpdateDto.UserName;
        user.Email = userUpdateDto.Email;

        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Something wrong with updating user with id : {id}");
        }
    }   
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString())
            ?? throw new EntityNotFoundException($"User with id : {id} is not found!");
        var result = await _userManager.DeleteAsync(user);
        
        return result.Succeeded ? true 
            : throw new InvalidOperationException($"Something wrong with deleting user with id : {id}");
    }
}
