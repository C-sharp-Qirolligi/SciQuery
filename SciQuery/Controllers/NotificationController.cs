using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification(string userId, string message)
        {
            await _notificationService.NotifyUser(userId, message);
            return Ok("Notification sent");
        }
    }
}
