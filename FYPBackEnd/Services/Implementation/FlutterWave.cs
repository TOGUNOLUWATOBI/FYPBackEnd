using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using RestSharp;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;

namespace FYPBackEnd.Services.Implementation
{
    public class FlutterWave : IFlutterWave
    {
        public FlutterWaveSettings settings;
        public ILogger<FlutterWave> log;
        private readonly ApplicationDbContext context;

        public FlutterWave(IOptions<FlutterWaveSettings> settings, ILogger<FlutterWave> log, ApplicationDbContext context)
        {
            this.settings = settings.Value;
            this.log = log;
            this.context = context;
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
            string createPaymentSubaccountUri = string.Concat(settings.BaseUrl, settings.PaymentSubaccount);

            var response = new CreatePaymentSubaccountResponseModel();
            var client = new RestClient(createPaymentSubaccountUri);
            var req = new RestRequest(Method.POST);

            model.bank_code = "232";
            model.country = "NG";
            req.AddJsonBody(model);
            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            

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


        public async Task<ApiResponse> GetFees(int amount)
        {
            string getFeesUri = string.Concat(settings.BaseUrl, settings.Fees);

            var response = new FeesResponseModel();
            var client = new RestClient(getFeesUri);
            var req = new RestRequest(Method.GET);

            req.AddQueryParameter("amount", amount.ToString());
            req.AddQueryParameter("currency", "NGN");

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            

            var resp = await client.ExecuteAsync(req);

            if (resp != null)
            {
                if (resp.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<FeesResponseModel>(resp.Content);
                    //log information gotten from flutterwave
                    log.LogInformation("Flutterwave Transfer fee fetched", response);
                    return ReturnedResponse.SuccessResponse("flutterwave Transfer fee fetched", response, StatusCodes.Successful);
                }
            }
            return ReturnedResponse.ErrorResponse("flutterwave Transfer fee couldn't be fetched", response, StatusCodes.ThirdPartyError);
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

        public async Task<ApiResponse> GetBillCategories (string biller_code, string data_bundle)
        {
            var response = new GetBillCategoriesResponseModel();
            var getBillCategoriesUri = string.Concat(settings.BaseUrl, settings.GetBillCategories);
            var client = new RestClient(getBillCategoriesUri);
            var req = new RestRequest(Method.GET);

            req.AddHeader("Authorization", $"Bearer {settings.SecretKey}");
            if(!string.IsNullOrEmpty(biller_code))
            {
                req.AddQueryParameter("biller_code", biller_code);
            }
            if (!string.IsNullOrEmpty(data_bundle))
            {                         
                req.AddQueryParameter("data_bundle", data_bundle);
            }
            req.AddQueryParameter("country", "NG");
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


        public async Task<ApiResponse> ProcessWebhook(WebhookRequest model)
        {

            var fw = new FlutterwaveWebhook()
            {
                webhook = model.ToString()
            };

            await context.FW.AddAsync(fw);
            await context.SaveChangesAsync();
            //todo: finish up on this
            if(model.Event == "transfer.completed")
            {

                //this is for both transfer in and successful transfer out
                if(model.data.status == "SUCCESSFUL")
                {
                    var transaction = await context.Transactions.FirstOrDefaultAsync(x=> x.Reference == model.data.reference);
                    if(transaction != null)
                    {
                        //update transaction to completed
                        transaction.Status = TransactionStatus.Completed.ToString();
                        context.Update(transaction);
                        await context.SaveChangesAsync();
                        //todo: push notification that the transaction has been succesful and should reach the beneficiary soon/immediately
                        return ReturnedResponse.SuccessResponse("Webhook successful", null, StatusCodes.Successful);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.data.complete_message))
                        {
                            var account = await context.Accounts.FirstOrDefaultAsync(x => x.ThirdPartyReference == model.data.reference);
                            if (account != null)
                            {
                                var fee = 1.4 * model.data.amount / 100;
                                //create new transaction record
                                var transactionn = new Transaction()
                                {
                                    Amount = model.data.amount,
                                    LastModifiedDate = DateTime.Now,
                                    CreationDate = DateTime.Now,
                                    BalanceBeforeTransaction = account.Balance,
                                    BalanceAfterTransaction = account.Balance + model.data.amount - (decimal)fee,
                                    Description = model.data.narration,
                                    ThirdPartyReference = model.data.reference,
                                    Reference = Guid.NewGuid().ToString(),
                                    id = Guid.NewGuid(),
                                    UserId = account.UserId,
                                    TransactionType = TransactionType.CashOut.ToString(),
                                    Status = TransactionStatus.Successful.ToString(),
                                    PostingType = PostingType.Cr.ToString(),
                                    Beneficiary = "NGN Wallet Funded  |  Wallet Funds",
                                    BeneficiaryBank = model.data.bank_name,
                                    BeneficiaryAccountNumber = model.data.account_number,
                                    BeneficiaryBankCode = model.data.bank_code
                                };

                                //save the transaction
                                context.Add(transactionn);
                                

                                // increase wallet balance
                                account.Balance = account.Balance + model.data.amount - (decimal)fee;
                                context.Update(account);

                                await context.SaveChangesAsync();

                                //add push notification to the user that the transaction was successful
                            }
                        }
                    }
                }


                // this is for failed transfer out of an account
                if(model.data.status == "FAILED")
                {
                    var transaction = await context.Transactions.FirstOrDefaultAsync(x => x.Reference == model.data.reference);
                    if (transaction != null)
                    {
                        //todo: push notification that the transaction has been reversed               /// let see how it goes
                        //todo: create reversal function in utility so as to avoid depency injection
                        return ReturnedResponse.SuccessResponse("Webhook successful", null, StatusCodes.Successful);
                    }
                }

                return ReturnedResponse.ErrorResponse("Something went wrong somwehere", model, StatusCodes.ThirdPartyError);
            }

            //todo: notify admin that an issue occured in the process and should check the database for the issue
            return ReturnedResponse.SuccessResponse("Webhook successful", model, StatusCodes.Successful);

        }

        public async Task<ApiResponse> AccountNameVerification(AccountNameVerificationModel model)
        {
            var AccountNameUri = string.Concat(settings.BaseUrl, settings.AccountNameVerifcation);

            AccountNameVerificationResponse response = new AccountNameVerificationResponse();
            var client = new RestClient(AccountNameUri);
            var req = new RestRequest(Method.POST);

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
