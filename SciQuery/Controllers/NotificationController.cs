using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciQuery.Infrastructure.Persistance.DbContext;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(NotificationService notificationService,SciQueryDbContext context) : ControllerBase
    {
        private readonly NotificationService _notificationService = notificationService;
        private readonly SciQueryDbContext _context = context;

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification(string userId, string message)
        {
            await _notificationService.NotifyUser(userId, message);
            return Ok("Notification sent");
        }

        [HttpPost("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkNotificationAsRead(id);
            return NoContent();
        }
    }
}
