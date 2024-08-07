using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.User;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;

public class UserService : IUserService
{
    private readonly SciQueryDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(SciQueryDbContext context ,UserManager<User> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PaginatedList<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToPaginatedList<UserDto, User>(_mapper.ConfigurationProvider, 1, 15);
        return users;
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        //var user = await _userManager.FindByIdAsync(id)
        //    ?? throw new EntityNotFoundException($"User with id : {id} is not found!");

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
            throw new InvalidOperationException($"Something wrong with creating user");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task UpdateAsync(string id, UserForUpdatesDto userUpdateDto)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new EntityNotFoundException($"User with id : {id} is not found!");

        user.UserName = userUpdateDto.UserName;
        user.Email = userUpdateDto.Email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Something wrong with updating user with id : {id}");
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        //var user = await _context.Users
        //    .FirstOrDefaultAsync(x => x.Id == id);
        //if (user == null)
        //{
        //    return false;
        //}
        //_context.Users.Remove(user);
        //await _context.SaveChangesAsync();
        //return true;
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Xatolikni loglash
            // _logger.LogError(ex, "Error occurred while deleting user.");
            throw; // Xatolikni yuqoriga qaytarish
        }
    }
}
