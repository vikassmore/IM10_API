using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IM10.API.Hubs
{
    public class NotificationsHubService : Hub<INotificationsHubService>
    {
        public async Task NewContentNotification( long playerId, long contentId, string title, string description)
        {
            await Clients.Group(playerId.ToString()).NewContentNotification( playerId, contentId, title, description);
        }

        public async Task NewSMSNotification(long playerId, long contentId, long commentId, string message)
        {
            await Clients.Group(playerId.ToString()).NewSMSNotification(playerId, contentId, commentId, message);
        }
        public async Task TrendingNotification(long playerId, long contentId, string title, string description)
        {
            await Clients.Group(playerId.ToString()).TrendingNotification(playerId, contentId, title, description);
        }
        public async Task<string> GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
