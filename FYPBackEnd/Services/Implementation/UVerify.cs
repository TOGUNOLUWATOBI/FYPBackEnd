using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.Models.UVerify.Request;
using FYPBackEnd.Data.Models.UVerify.Requests;
using FYPBackEnd.Data.Models.UVerify.Responses;
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
    public class UVerify :IUVerify
    {
        private readonly UVerifySettings settings;
        public ILogger<FlutterWave> log;


        public UVerify(IOptions<UVerifySettings> settings, ILogger<FlutterWave> log)
        {
            this.settings = settings.Value;
            this.log = log;
        }


        public async Task<ApiResponse> VerifyBVN(BvnVerificationRequestModel model)
        {
            string verifyBvnUri = string.Concat(settings.BaseUrl, settings.BVN);

            BvnverifyResponseModel response = new BvnverifyResponseModel();
            var client = new RestClient(verifyBvnUri);
            var req = new RestRequest(Method.POST);

            model.isSubjectConsent = true;

            req.AddHeader("token", $"{settings.ApiKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<BvnverifyResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("User kyc(you verify): bvn verification successful", response);
                    return ReturnedResponse.SuccessResponse("bvn verification successful", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("youverify couldn't successfully verify user bvn", response, StatusCodes.ThirdPartyError);
        }


        public async Task<ApiResponse> VerifyNin(NinVerificationRequestModel model)
        {
            string verifyNinUri = string.Concat(settings.BaseUrl, settings.NIN);
            model.isSubjectConsent = true;
            NinVerifyResponseModel response = new NinVerifyResponseModel();
            var client = new RestClient(verifyNinUri);
            var req = new RestRequest(Method.POST);

            req.AddHeader("token", $"{settings.ApiKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);
            return ReturnedResponse.SuccessResponse("bbb", resp.Content, StatusCodes.Successful);
            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<NinVerifyResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("User kyc(you verify): NIN verification successful", response);
                    return ReturnedResponse.SuccessResponse("NIN verification successful", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("youverify couldn't successfully verify user NIN", response, StatusCodes.ThirdPartyError);
        }


        public async Task<ApiResponse> VerifyPassport(PassportVerificationRequestModel model)
        {
            string verifyPassportUri = string.Concat(settings.BaseUrl, settings.Passport);

            PassportVerifyResponseModel response = new PassportVerifyResponseModel();
            var client = new RestClient(verifyPassportUri);
            var req = new RestRequest(Method.POST);

            model.isSubjectConsent = true;

            req.AddHeader("token", $"{settings.ApiKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<PassportVerifyResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("User kyc(you verify): Passport verification successful", response);
                    return ReturnedResponse.SuccessResponse("Paspport verification successful", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("youverify couldn't successfully verify user passport", response, StatusCodes.ThirdPartyError);
        }


        public async Task<ApiResponse> VerifyDriversLicense(DriverLicesnseVerificationRequestModel model)
        {
            string verifyDriverLicenseUri = string.Concat(settings.BaseUrl, settings.DriverLicense);

            DriversLicenseVerifyResponseModel response = new DriversLicenseVerifyResponseModel();
            var client = new RestClient(verifyDriverLicenseUri);
            var req = new RestRequest(Method.POST);

            model.isSubjectConsent = true;

            req.AddHeader("token", $"{settings.ApiKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<DriversLicenseVerifyResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("User kyc(you verify): Passport verification successful", response);
                    return ReturnedResponse.SuccessResponse("Paspport verification successful", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("youverify couldn't successfully verify user passport", response, StatusCodes.ThirdPartyError);
        }

    }
}
