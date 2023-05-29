using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IWebhookService
    {
        Task<ApiResponse> FWHandleWebhook(WebhookRequest request, string theSecretHash);
    }
}
