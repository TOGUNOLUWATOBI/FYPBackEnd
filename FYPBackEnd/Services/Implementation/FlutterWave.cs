using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
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

        public async Task<ApiResponse> CreatePayoutSubaccount(CreatePaymentSubaccountRequestModel model)
        {
            string createPaymentSubaccountUri = string.Concat(settings.BaseUrl, settings.VirtualAccount);

            var response = new CreatePaymentSubaccountResponseModel();
            var client = new RestClient(createPaymentSubaccountUri);
            var req = new RestRequest(Method.POST);

            model.bank_code = "232";
            model.country = "NGN";

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<CreatePaymentSubaccountResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Create Payment Subaccount", response);
                    return ReturnedResponse.SuccessResponse("flutterwave Payment Subaccount created", response, StatusCodes.Successful);
                }
            }
            return ReturnedResponse.ErrorResponse("flutterwave Payment Subaccount couldn't be created", response, StatusCodes.ThirdPartyError);
        }


        public async Task<ApiResponse> GetPaymentSubaccountBalance(string thirdpartyReference)
        {
            if (string.IsNullOrEmpty(thirdpartyReference))
                return ReturnedResponse.ErrorResponse("thirdpartyReference Can't be null or empty", null, StatusCodes.GeneralError);

            var response = new GetVirtualAccountResponseModel();
            string getPaymentSubaccountBalanceUri = string.Concat(settings.BaseUrl, settings.PaymentSubaccount, "/", thirdpartyReference,"/","balances");

            var client = new RestClient(getPaymentSubaccountBalanceUri);
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
                    return ReturnedResponse.SuccessResponse("flutterwave payment subaccount balance", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave vitrual account couldn't be retrieved.", response, StatusCodes.ThirdPartyError);
        }

        public async Task<ApiResponse> GetPaymentSubaccount(string thirdpartyReference)
        {
            if (string.IsNullOrEmpty(thirdpartyReference))
                return ReturnedResponse.ErrorResponse("thirdpartyReference Can't be null or empty", null, StatusCodes.GeneralError);

            var response = new GetVirtualAccountResponseModel();
            string getPaymentSubaccountUri = string.Concat(settings.BaseUrl, settings.PaymentSubaccount, "/", thirdpartyReference);

            var client = new RestClient(getPaymentSubaccountUri);
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
                    return ReturnedResponse.SuccessResponse("flutterwave payment subaccount balance", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave vitrual account couldn't be retrieved.", response, StatusCodes.ThirdPartyError);
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
            var response = new InitiateTransferResponseModel();

            var initiateTransferUri = string.Concat(settings.BaseUrl, settings.Transfers);
            var client = new RestClient(initiateTransferUri);
            var req = new RestRequest(Method.POST);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddJsonBody(model);

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<InitiateTransferResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Transfer Initiated", response);
                    return ReturnedResponse.SuccessResponse("flutterwave transfer initiated", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave transfer couldn't be initiated.", response, StatusCodes.ThirdPartyError);
        }

        public async Task<ApiResponse> GetBillCategories ()
        {
            var response = new GetBillCategoriesResponseModel();
            var getBillCategoriesUri = string.Concat(settings.BaseUrl, settings.GetBillCategories);
            var client = new RestClient(getBillCategoriesUri);
            var req = new RestRequest(Method.GET);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<GetBillCategoriesResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Get VIrtual Account", response);
                    return ReturnedResponse.SuccessResponse("flutterwave bill categories", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave bill categories couldn't be retrieved.", response, StatusCodes.ThirdPartyError);
        }

        public async Task<ApiResponse> PayBill(PayBillRequestModel model)
        {
            var response = new PayBillResponseModel();

            var PayBIllPaymentUri = string.Concat(settings.BaseUrl, settings.PayBill);
            var req =   new RestRequest(Method.POST);
            var client = new RestClient(PayBIllPaymentUri);
            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<PayBillResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Get VIrtual Account", response);
                    return ReturnedResponse.SuccessResponse("flutterwave bills payment initiated", response, StatusCodes.Successful);
                }
            }

            return ReturnedResponse.ErrorResponse("flutterwave bills payment  couldn't be initiated.", response, StatusCodes.ThirdPartyError);
        }



        // TODO: properly test this endpoint wasn't working with postman during test.
        public async Task<ApiResponse> ValidateBillPayment(ValidateBillRequestModel model)
        {
            var response = new ValidateBillPaymentResponseModel();

            var validateBillPaymentUri = string.Concat(settings.BaseUrl, settings.ValidateBillPayment,$"/:{model.item_code}/validate");
            var client = new RestClient(validateBillPaymentUri);
            var req = new RestRequest(Method.GET);
            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            req.AddJsonBody(model);
            var resp = await client.ExecuteAsync(req);
            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<ValidateBillPaymentResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Validate Bill Payment", response);
                    return ReturnedResponse.SuccessResponse("flutterwave bill payment validated", response, StatusCodes.Successful);
                }
            }
            return ReturnedResponse.ErrorResponse("flutterwave bill payment couldn't be validated", response, StatusCodes.ThirdPartyError);
            
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
