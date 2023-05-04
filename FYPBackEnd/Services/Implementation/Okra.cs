using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.Models.Okra;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Implementation
{
    public class Okra : IOkra
    {
        public OkraSettings settings;
        public ILogger<Okra> log;

        public Okra(IOptions<OkraSettings> settings, ILogger<Okra> log)
        {
            this.settings = settings.Value;
            this.log = log;
        }

        public async Task<ApiResponse> GetBvnDetailsById(GetBvnByIdRequestModel model)
        {
            string getBvnDetailsUri = string.Concat(settings.BaseUrl, settings.GetBvn);

            var response = new GetBVNResponseModel();
            var client = new RestClient(getBvnDetailsUri);
            var req = new RestRequest(Method.POST);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddHeader("content-type", "application/json");
            req.AddHeader("accept", "application/json");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<GetBVNResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Bvn Details retrieved", response);
                    return ReturnedResponse.SuccessResponse("Okra Bvn Details retrieved", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("Okra Bvn Details couldn't be retrieved", response, StatusCodes.ThirdPartyError);
        }


        public async Task<ApiResponse> GetBvnDetailsByBvn(GetBvnByBvnRequestModel model)
        {
            string getBvnDetailsUri = string.Concat(settings.BaseUrl, settings.BvnVerify);

            var response = new GetBVNResponseModel();
            var client = new RestClient(getBvnDetailsUri);
            var req = new RestRequest(Method.POST);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddHeader("content-type", "application/json");
            req.AddHeader("accept", "application/json");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<GetBVNResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Okra Bvn verification", response);
                    return ReturnedResponse.SuccessResponse("Okra Bvn verification", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("Okra Bvn verification couldn't be completed", response, StatusCodes.ThirdPartyError);
        }
    }
}
