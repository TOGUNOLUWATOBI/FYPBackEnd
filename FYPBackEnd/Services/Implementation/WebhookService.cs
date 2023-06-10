using epAgentAuthentication.Services;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Implementation
{
    public class WebhookService : IWebhookService 
    {
        private readonly IFlutterWave flutterwave;
        private readonly FlutterWaveSettings flutterWaveSettings;
        private readonly UVerifySettings uVerifySettings;
        private readonly OkraSettings okraSettings;

        public WebhookService(IFlutterWave flutterwave, IOptions<OkraSettings> okraSettings, IOptions<UVerifySettings> uVerifySettings, IOptions<FlutterWaveSettings> flutterWaveSettings)
        {
            this.flutterwave = flutterwave;
            this.okraSettings = okraSettings.Value;
            this.uVerifySettings = uVerifySettings.Value;
            this.flutterWaveSettings = flutterWaveSettings.Value;
        }

        public async Task<ApiResponse> FWHandleWebhook(WebhookRequest request)
        {
            var response = await flutterwave.ProcessWebhook(request);
            return response;
        }
    }
}
