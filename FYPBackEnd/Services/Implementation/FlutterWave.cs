using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Implementation
{
    public class FlutterWave : IFlutterWave
    {
        public FlutterWaveSettings settings;
        public ILogger<FlutterWave> log;

        public FlutterWave(IOptions<FlutterWaveSettings> settings, ILogger<FlutterWave> log)
        {
            this.settings = settings.Value;
            this.log = log;
        }

        public async Task<ApiResponse> CreateVirtualStaticAccount(CreateVIrtualRequestModel model)
        {
            string createVirtualAccountUri = string.Concat(settings.BaseUrl, settings.VirtualAccount);

            CreateVIrtualResponseModel response= new CreateVIrtualResponseModel();
            var client = new RestClient(createVirtualAccountUri);
            var req = new RestRequest(Method.POST);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if(resp != null)
            {
                if(resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<CreateVIrtualResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Create VIrtual Account", response);
                    return ReturnedResponse.SuccessResponse("flutterwave vitrual account created", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave vitrual account couldn't be created", response, StatusCodes.ThirdPartyError);

        }

        public async Task<ApiResponse> GetVirtualStaticAccount (string orderRef)
        {
            if (string.IsNullOrEmpty(orderRef))
                return ReturnedResponse.ErrorResponse("OrderRef Can't be null or empty", null, StatusCodes.GeneralError);

            var response = new GetVirtualAccountResponseModel();
            string getVirtualAccountUri = string.Concat(settings.BaseUrl,settings.VirtualAccount,"/",orderRef);

            var client = new RestClient(getVirtualAccountUri);
            var req = new RestRequest(Method.GET);
            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<GetVirtualAccountResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Get VIrtual Account", response);
                    return ReturnedResponse.SuccessResponse("flutterwave vitrual details", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave vitrual account couldn't be retrieved.", response, StatusCodes.ThirdPartyError);

        }

        public async Task<ApiResponse> InitiateTransfer (InitiateTransferRequestModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> GetAllBanks()
        {
            var getBanksUri = string.Concat(settings.BaseUrl, settings.GetBanks);

            var response = new BankResponseModel();
            var client = new RestClient(getBanksUri);
            var req = new RestRequest(Method.GET);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<BankResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("gets all banks", response);
                    return ReturnedResponse.SuccessResponse("get all banks", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave couldn't retrieve all banks", response, StatusCodes.ThirdPartyError);
        }

        public async Task<ApiResponse> AccountNameVerification(AccountNameVerificationModel model)
        {
            var AccountNameUri = string.Concat(settings.BaseUrl, settings.AccountNameVerifcation);

            AccountNameVerificationResponse response = new AccountNameVerificationResponse();
            var client = new RestClient(AccountNameUri);
            var req = new RestRequest(Method.GET);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<AccountNameVerificationResponse>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Account Name details", response);
                    return ReturnedResponse.SuccessResponse("Account name details", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave couldn't get account name details", response, StatusCodes.ThirdPartyError);
        }

    }
}
