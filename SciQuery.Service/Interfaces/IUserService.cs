using SciQuery.Service.DTOs.User;
using SciQuery.Service.Pagination.PaginatedList;

namespace SciQuery.Service.Interfaces;
public interface IUserService
{
    Task<PaginatedList<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> CreateAsync(UserForCreateDto userCreateDto);
    Task UpdateAsync(string id, UserForUpdatesDto userUpdateDto);
    Task<bool> DeleteAsync(string id);
}
