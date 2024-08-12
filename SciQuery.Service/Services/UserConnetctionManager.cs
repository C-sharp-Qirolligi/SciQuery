    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.Services
{
    public interface IUserConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string connectionId);
        List<string> GetConnections(string userId);
    }

    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly Dictionary<string, List<string>> _userConnections = new();

        public void AddConnection(string userId, string connectionId)
        {
            lock (_userConnections)
            {
                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = new List<string>();
                }

                _userConnections[userId].Add(connectionId);
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (_userConnections)
            {
                foreach (var userId in _userConnections.Keys)
                {
                    if (_userConnections[userId].Contains(connectionId))
                    {
                        _userConnections[userId].Remove(connectionId);
                        if (!_userConnections[userId].Any())
                        {
                            _userConnections.Remove(userId);
                        }
                        break;
                    }
                }
            }
        }

        public List<string> GetConnections(string userId)
        {
            _userConnections.TryGetValue(userId, out var connections);
            return connections ?? new List<string>();
        }
    }
}
