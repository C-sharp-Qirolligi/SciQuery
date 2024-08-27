using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.Interfaces;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService,SciQueryDbContext context) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly SciQueryDbContext _context = context;

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] Notification notification)
        {
            await _notificationService.NotifyUser(notification);
            return Ok("Notification sent");
        }

        
    }
}
