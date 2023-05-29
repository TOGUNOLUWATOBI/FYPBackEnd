using FYPBackEnd.Data.Models.Notification;
using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponse> SendNotification(NotificationModel notificationModel);
    }
}
