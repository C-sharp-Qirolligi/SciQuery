using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SciQuery.Service.Notifications;
public class NotificationUserHub : Hub
{
    private readonly IUserConnectionManager _userConnectionManager;
    public NotificationUserHub(IUserConnectionManager userConnectionManager)
    {
        _userConnectionManager = userConnectionManager;
    }
    public string GetConnectionId()
    {
        var httpContext = this.Context.GetHttpContext();

        var userId = (Context.User?.FindFirst("sub")?.Value)
            ?? throw new UnauthorizedAccessException("User is not authenticated."); // or use "userId" based on your claim type

        _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);
        return Context.ConnectionId;
    }
    //Called when a connection with the hub is terminated.
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        //get the connectionId
        var connectionId = Context.ConnectionId;
        _userConnectionManager.RemoveUserConnection(connectionId);
        var value = await Task.FromResult(0);
    }
}
