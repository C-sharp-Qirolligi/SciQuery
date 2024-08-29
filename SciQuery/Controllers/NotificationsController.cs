using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SciQuery.Domain.Entities;
using SciQuery.Domain.UserModels;
using SciQuery.Service.Interfaces;
using SciQuery.Service.QueryParams;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(
        INotificationService notificationService,
        UserManager<User> userManager
        ) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllByUserId([FromQuery] NotificationQueryParameters queryParameters)
        {
            var userId = _userManager.GetUserId(User); 
            
            if(userId is null)
            {
                NotFound();
            }

            var notifications =  await _notificationService.GetNotificationsByUserId(userId,queryParameters);
            return Ok(notifications);
        }
        
        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] Notification notification)
        {
            await _notificationService.NotifyUser(notification);
            return Ok("Notification sent");
        }

        
    }
}
