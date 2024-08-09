using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Service.DTOs.User;
using SciQuery.Service.Interfaces;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserForCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateAsync(userCreateDto);
            if (createdUser == null)
            {
                return BadRequest("User could not be created.");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        [HttpPost("UploadImages")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            var result = await _userService.CreateImage(file);
            return Ok(result);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserForUpdatesDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.UpdateAsync(id, userUpdateDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            //var result = await _userService.DeleteAsync(id);
            //if (!result)
            //{
            //    return NotFound();
            //}

            //return NoContent();
            try
            {
                var result = await _userService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(); // Foydalanuvchi topilmadi
                }

                return NoContent(); // O'chirish muvaffaqiyatli
            }
            catch (Exception ex)
            {
                // Xatolikni loglash
                // _logger.LogError(ex, "An error occurred while deleting the user.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
