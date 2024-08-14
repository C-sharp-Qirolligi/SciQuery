namespace SciQuery.Service.Notifications;

public interface IUserConnectionManager
{
    void KeepUserConnection(string userId, string connectionId);
    void RemoveUserConnection(string connectionId);
    List<string> GetUserConnections(string userId);
}
