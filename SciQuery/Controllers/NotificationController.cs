using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SciQuery.Service.Hubs;

namespace SciQuery.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController(IHubContext<NotificationHub, INotificationClient> hubContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(string notification)
    {
        await hubContext.Clients.All.ReceiveNotification($"->->{notification}");
        return Ok();
    }
}
