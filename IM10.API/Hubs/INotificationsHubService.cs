using System.Collections.Generic;
using System.Threading.Tasks;

namespace IM10.API.Hubs
{
    public interface INotificationsHubService
    {
        Task NewContentNotification( long playerId, long contentId, string title, string description);
        Task NewSMSNotification(long playerId, long contentId, long commentId, string message);
        Task TrendingNotification(long playerId, long contentId, string title, string description);

    }
}
