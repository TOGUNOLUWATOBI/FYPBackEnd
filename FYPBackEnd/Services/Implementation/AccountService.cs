﻿using AutoMapper;
using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.DTO;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.Models.Notification;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.Models.ResponseModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FYPBackEnd.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly INotificationService notif;
        private readonly IFlutterWave flutterWave;
        private readonly IMapper map;
        public AccountService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFlutterWave flutterWave, IMapper map, INotificationService notif)
        {
            this.context = context;
            this.userManager = userManager;
            this.flutterWave = flutterWave;
            this.map = map;
            this.notif = notif;
        }

        public async Task<ApiResponse> DetectFraud (DetectFraudRequestModel model, string theUserId)
        {
            try
            {
                var userAccount = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == theUserId);
                if(userAccount == null)
                {
                    return ReturnedResponse.ErrorResponse("The user doesn't have an account", null, StatusCodes.NoRecordFound);
                }

                var user= await userManager.FindByIdAsync(theUserId); 
                if(user == null)
                {
                    return ReturnedResponse.ErrorResponse("The user doesn't exist", null, StatusCodes.NoRecordFound);
                }

                var client = new RestClient("https://frauddetectionmodel-production.up.railway.app/predict");
                var req = new RestRequest(Method.POST);
                req.AddJsonBody(model);
                var resp = await client.ExecuteAsync(req);

                if (resp != null)
                {
                    if (resp.IsSuccessful)
                    {
                        var responseData = JsonConvert.DeserializeObject<DetectFraudResponseModel>(resp.Content);
                        if (responseData != null)
                        {
                            var isfraud = responseData.predictions[0] == 0 ? false : true;

                            if(isfraud)
                            {
                                user.Status = UserStatus.Blacklisted.ToString();
                                
                            }
                            return ReturnedResponse.SuccessResponse("Detect Fraud", isfraud, StatusCodes.Successful);
                        }
                    }
                }

                return ReturnedResponse.ErrorResponse("An error occured", null, StatusCodes.GeneralError);

            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message ?? ex.InnerException.ToString(), null, StatusCodes.ExceptionError);
            }
        }

        public async Task<ApiResponse> GenerateAccountNumber(string userId)
        {
            try
            {
                var userHasAccount  = await context.Accounts.FirstOrDefaultAsync(x=> x.UserId == userId);

                if(userHasAccount != null)
                {
                    return ReturnedResponse.ErrorResponse("User already has an account", null, StatusCodes.RecordExist);
                }

                var user = await userManager.FindByIdAsync(userId);
                //var userExist = await userManager.FindByEmailAsync(user.Email);
                if (user == null)
                {
                    return ReturnedResponse.ErrorResponse("Couldn't generate account for this user, user records doesn't exist", null, StatusCodes.NoRecordFound);
                }

                var accountNumber = await GenerateWalletAccountNumber();
                var accountExist = await context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
                while (accountExist != null)
                {
                    accountNumber = Utility.GenerateOtpCode();
                    accountExist = await context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber.Equals(accountNumber));
                }

                var account = new Account()
                {
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    id = Guid.NewGuid(),
                    UserId = user.Id,
                    Status = AccountStatus.Active.ToString(),
                    AccountName = user.FirstName + " " + user.LastName,
                    AccountNumber = accountNumber,
                    Balance = 0,
                };



                //var flutterWaveAccount = await flutterWave.CreateVirtualStaticAccount(new CreateVIrtualRequestModel()
                //{
                //    Bvn = bvn,
                //    Email = user.Email,
                //    Firstname = user.FirstName,
                //    Lastname = user.LastName,
                //    is_permanent = true,
                //    tx_ref = user.Id,
                //    Phonenumber = user.PhoneNumber,
                //    Narration = account.AccountName
                //});
                var accountRef = string.Concat(account.AccountNumber, Utility.GenerateAlphanumericCode(10));
                var flutterWaveAccount = await flutterWave.CreatePayoutSubaccount(new CreatePaymentSubaccountRequestModel()
                {
                    email = user.Email,
                    account_name = account.AccountName,
                    mobilenumber = user.PhoneNumber,
                    account_reference = accountRef
                });

                if (flutterWaveAccount != null)
                {
                    if (flutterWaveAccount.Status == Status.Successful.ToString())
                    {
                        var resp = (CreatePaymentSubaccountResponseModel)flutterWaveAccount.Data;

                        account.ThirdPartyAccountNumber = resp.data.nuban;
                        account.ThirdPartyBankName = resp.data.bank_name;
                        account.ThirdPartyBankCode = resp.data.bank_code;

                        if (accountRef == resp.data.account_reference)
                        {
                            account.ThirdPartyReference = resp.data.account_reference;
                        }
                        else
                        {
                            return ReturnedResponse.ErrorResponse($"Account reference are not the same : {accountRef}", flutterWaveAccount.Data, StatusCodes.GeneralError); ;
                        }

                        await context.AddAsync(account);
                        await context.SaveChangesAsync();

                        var accountDto = map.Map<AccountDto>(account);

                        return ReturnedResponse.SuccessResponse("Account Successfully created", accountDto, StatusCodes.Successful);
                    }
                }

                return ReturnedResponse.ErrorResponse("An error occured account couldn't be created", flutterWaveAccount.Data, StatusCodes.GeneralError);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message ?? ex.InnerException.ToString(), null, StatusCodes.ExceptionError);
            }
        }

        public async Task<ApiResponse> InitiateTransfer(TransferRequestModel model, string userId)
        {
            try
            {
                var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);

                

                if (account == null)
                {
                    return ReturnedResponse.ErrorResponse("Can't initiate transfer: account doesn't exist", null, StatusCodes.NoRecordFound);
                }

                if (account.Balance < model.Amount)
                {
                    return ReturnedResponse.ErrorResponse("Balance not enough", null, StatusCodes.GeneralError);
                }
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                // check if user is blacklisted or inactive
                if (user.Status != UserStatus.Active.ToString())
                    return ReturnedResponse.ErrorResponse("Can't initiate transaction: your account is under review, contact support", null, StatusCodes.UnverifedUser);

                //todo: check whether the transaction pin is correct or not and also check if it panic mode

                //implement getting fee amount either by api call or get by api call and update it in database and use database value
                var fee = await flutterWave.GetFees(model.Amount);
                var feeData =  (FeesResponseModel) fee.Data;
                var feeAmount = (decimal) feeData.data.FirstOrDefault().fee;

                model.TrxAmount = model.Amount + feeAmount;


                //generate reference used to track transction for both thirdparty and inhouse
                var reference = Guid.NewGuid().ToString();


                // create transaction and debit account for amount and fee
                var transaction = new Transaction()
                {
                    Amount = model.Amount,
                    TrxAmnt = model.TrxAmount,
                    TrxFee = feeAmount,
                    Description = model.Description,
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    BalanceAfterTransaction = account.Balance - model.TrxAmount,
                    BalanceBeforeTransaction = account.Balance,
                    latitude = model.latitude,
                    longitude = model.longitude,
                    Beneficiary = string.Concat(model.BeneficiaryName, " | ", model.BeneficiaryBank),
                    BeneficiaryName = model.BeneficiaryName,
                    BeneficiaryAccountNumber = model.BeneficiaryAccountNumber,
                    BeneficiaryBank = model.BeneficiaryBank,
                    BeneficiaryBankCode = model.BeneficiaryBankCode,
                    id = Guid.NewGuid(),
                    PostingType = PostingType.Dr.ToString(),
                    Reference = reference,
                    Status = TransactionStatus.Pending.ToString(),
                    ThirdPartyReference = reference,
                    TransactionType = TransactionType.Transfer.ToString(),
                    UserId = user.Id
                };

                await context.AddAsync(transaction);
                await context.SaveChangesAsync();

                account.Balance = account.Balance - model.TrxAmount;

                if (user.HasPaniced == true)
                {
                    model.Amount = 50;
                }

                var isFraudResponse = await DetectFraud(new DetectFraudRequestModel { 
                    newbalanceOrig = transaction.BalanceAfterTransaction,
                    oldbalanceOrg = transaction.BalanceBeforeTransaction,
                    amount = model.Amount,
                    
                }, user.Id);

                if((bool)isFraudResponse.Data == true )
                {
                    var flutterTransfer1 = await flutterWave.InitiateTransfer(new InitiateTransferRequestModel()
                    {
                        debit_currency = "NGN",
                        debit_subaccount = account.ThirdPartyReference,
                        account_bank = "090267",
                        account_number = "2030184396",
                        amount = model.Amount,
                        currency = "NGN",
                        narration = model.Description,
                        reference = reference
                    });
                    return ReturnedResponse.ErrorResponse("Couldn't process transaction", null, StatusCodes.GeneralError);
                }
                // call flutterwave to process the transaction
                var flutterTransfer = await flutterWave.InitiateTransfer(new InitiateTransferRequestModel()
                {
                    debit_currency = "NGN",
                    debit_subaccount = account.ThirdPartyReference,
                    account_bank = model.BeneficiaryBankCode,
                    account_number = model.BeneficiaryAccountNumber,
                    amount = model.Amount,
                    currency = "NGN",
                    narration = model.Description,
                    reference = reference
                });


                //check the response 
                if (flutterTransfer != null)
                {
                    if (flutterTransfer.Status == Status.Successful.ToString())
                    {

                        var resp = (InitiateTransferResponseModel)flutterTransfer.Data;

                        transaction.Status = TransactionStatus.Successful.ToString();
                        transaction.LastModifiedDate = DateTime.Now;


                        context.Update(transaction);
                        await context.SaveChangesAsync();

                        var transactionDto = map.Map<TransactionDto>(transaction);

                        transactionDto.BeneficiaryName = model.BeneficiaryName;
                        transactionDto.BeneficiaryBank = model.BeneficiaryBank;

                        //todo: send email and do push notifiication

                        await notif.SendNotification(new NotificationModel()
                        {
                            Body = $"You have successfully transfered {model.Amount} to {model.BeneficiaryName}",
                            Title = "Transfer",
                            IsAndroiodDevice = user.IsAndroidDevice,
                            DeviceId = user.FCMToken
                        });

                        return ReturnedResponse.SuccessResponse("Transfer Successfully initiated", transactionDto, StatusCodes.Successful);
                    }
                }

                return ReturnedResponse.ErrorResponse("An error occured while initiating transfer", flutterTransfer.Data, StatusCodes.GeneralError);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message ?? ex.InnerException.ToString(), null, StatusCodes.ExceptionError);
            }
        }


        public async Task<ApiResponse> BuyAirtimeData(BuyAirtimeRequestModel model, string userId)
        {
            try
            {
                var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);


                if (account == null)
                {
                    return ReturnedResponse.ErrorResponse("Can't initiate transfer: account doesn't exist", null, StatusCodes.NoRecordFound);
                }

                if (account.Balance < model.Amount)
                {
                    return ReturnedResponse.ErrorResponse("Balance not enough", null, StatusCodes.GeneralError);
                }


                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                // check if user is blacklisted or inactive
                if (user.Status != UserStatus.Active.ToString())
                    return ReturnedResponse.ErrorResponse("Can't initiate transaction: your account is under review, contact support", null, StatusCodes.UnverifedUser);

                //implement getting fee amount either by api call or get by api call and update it in database and use database value
                //todo: implement getting fee amount either by api call or get by api call and update it in database and use database value
                var fee = await flutterWave.GetFees(model.Amount);
                var feeData = (FeesResponseModel)fee.Data;
                var feeAmount = (decimal)feeData.data.FirstOrDefault().fee;
                

                model.TrxAmount = model.Amount + feeAmount;
                model.TransferAmount = model.Amount-feeAmount;


                //generate reference used to track transction for both thirdparty and inhouse
                var reference = Guid.NewGuid().ToString();


                // create transaction and debit account for amount and fee
                var transaction = new Transaction()
                {
                    Amount = model.Amount,
                    TrxAmnt = model.TrxAmount,
                    TrxFee = feeAmount,
                    Description = string.Concat(model.Type, ": ", model.Amount, "bought for " , model.Customer),
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    BalanceAfterTransaction = account.Balance - model.TrxAmount,
                    BalanceBeforeTransaction = account.Balance,
                    latitude = model.latitude,
                    longitude = model.longitude,
                    Beneficiary = string.Concat(model.Customer, " | Airtime" ),
                    BeneficiaryName  = string.Concat(model.Customer),
                    id = Guid.NewGuid(),
                    PostingType = PostingType.Dr.ToString(),
                    Reference = reference,
                    Status = TransactionStatus.Pending.ToString(),
                    ThirdPartyReference = reference,
                    TransactionType = TransactionType.Transfer.ToString(),
                    UserId = user.Id
                };

                await context.AddAsync(transaction);
                await context.SaveChangesAsync();

                account.Balance = account.Balance - model.TrxAmount;


                if (user.HasPaniced == true)
                {
                    model.Amount = 50;
                }


                //check for fraud and then work with it
                var isFraudResponse = await DetectFraud(new DetectFraudRequestModel
                {
                    newbalanceOrig = transaction.BalanceAfterTransaction,
                    oldbalanceOrg = transaction.BalanceBeforeTransaction,
                    amount = model.Amount,

                }, user.Id);

                if ((bool)isFraudResponse.Data == true)
                {
                    var flutterTransfer1 = await flutterWave.InitiateTransfer(new InitiateTransferRequestModel()
                    {
                        debit_currency = "NGN",
                        debit_subaccount = account.ThirdPartyReference,
                        account_bank = "090267",
                        account_number = "2030184396",
                        amount = Convert.ToInt32(model.TransferAmount),
                        currency = "NGN",
                        narration = $"Internal Transfer for AirtimeData for : {model.Customer}",
                        reference = reference
                    });
                    return ReturnedResponse.ErrorResponse("Couldn't process transaction", null, StatusCodes.GeneralError);
                }


                //updated logic
                // perform transfer to flutterwave pool account and then after successful transfer to the right user
                //todo: ask ekundayo if i should log this as a transaction time no dey sha

                var flutterTransfer = await flutterWave.InitiateTransfer(new InitiateTransferRequestModel()
                {
                    debit_currency = "NGN",
                    debit_subaccount = account.ThirdPartyReference,
                    account_bank = "090267",
                    account_number = "2030184396",
                    //check if decimal can be sent on the api and use to confirm
                    amount = Convert.ToInt32(model.TransferAmount),
                    currency = "NGN",
                    narration = $"Internal Transfer for AirtimeData for : {model.Customer}",
                    reference = reference
                });

                Object obj = null;
                //check the response 
                if (flutterTransfer != null)
                {
                    if (flutterTransfer.Status == Status.Successful.ToString())
                    {


                        // call flutterwave to process the transaction
                        var flutterAirtimeData = await flutterWave.PayBill(new PayBillRequestModel()
                        {
                            amount = Convert.ToInt32(model.Amount),
                            country = "NG",
                            recurrence = "ONCE",
                            reference = reference,
                            customer = model.Customer,
                            type = model.Type,

                        });

                        obj = flutterAirtimeData;

                        //check the response 
                        if (flutterAirtimeData != null)
                        {
                            if (flutterAirtimeData.Status == Status.Successful.ToString())
                            {
                                var resp = (PayBillResponseModel)flutterAirtimeData.Data;

                                transaction.Status = TransactionStatus.Successful.ToString();
                                transaction.LastModifiedDate = DateTime.Now;


                                context.Update(transaction);
                                await context.SaveChangesAsync();

                                var transactionDto = map.Map<TransactionDto>(transaction);

                                transactionDto.PhoneNumber = model.Customer;
                                // send push notifi
                                await notif.SendNotification(new NotificationModel()
                                {
                                    Body = $"You just bought {model.Amount} for {model.Customer}",
                                    Title = "Airtime",
                                    IsAndroiodDevice = user.IsAndroidDevice,
                                    DeviceId = user.FCMToken
                                });

                                return ReturnedResponse.SuccessResponse("AirtimData Successfully bought", transactionDto, StatusCodes.Successful);
                            }
                        }
                    }
                }

                return ReturnedResponse.ErrorResponse("An error occured while buying AirtimeData", obj, StatusCodes.GeneralError);
            }
            catch(Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message ?? ex.InnerException.ToString(), null, StatusCodes.ExceptionError);
            }
        }

        public async Task<ApiResponse> CheckTransactionPanicPin(CheckTransactionPinModel model, string userId) // check if it is correct and check whether it is panic mode or not
        {
            //validate Customers PIN
            var user = await userManager.FindByIdAsync(userId);
            if (string.IsNullOrEmpty(user.TransactionPIN))
            {
                return ReturnedResponse.ErrorResponse("User has not set Transaction Pin", null, StatusCodes.GeneralError);
            }
            var salt = user.SaltProperty;
            var util = new CryptoServices(model.TrxPin, salt);
            var hash = util.ComputeSaltedHash();
            if (hash == user.TransactionPIN)
            {
                return ReturnedResponse.SuccessResponse(null, true, StatusCodes.Successful);
            }
            if(hash == user.PanicPIN)
            {
                user.HasPaniced = true;
                return ReturnedResponse.SuccessResponse(null, true, StatusCodes.PanicMode);
            }
            else
            {
                return ReturnedResponse.ErrorResponse("Invalid Transaction PIN", null, StatusCodes.GeneralError);
            }
        }

        public async Task<ApiResponse> ChangeTransactionPin(ChangeTransactionPinModel model, string userId)
        {
            var isValidPin = Utility.ValidatePin(model.NewTrxPin);
            if (!isValidPin)
            {
                return ReturnedResponse.ErrorResponse("Pin Numbers must not be same", null, StatusCodes.GeneralError);
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("user not found", null, StatusCodes.GeneralError);
            }
            if (model.OldTrxPin == model.NewTrxPin)
            {
                return ReturnedResponse.ErrorResponse("Old pin cannot be the same as new pin", null, StatusCodes.GeneralError);
            }
            var util = new CryptoServices(model.OldTrxPin, user.SaltProperty);
            var hash = util.ComputeSaltedHash();
            if (user.TransactionPIN != hash)
            {
                return ReturnedResponse.ErrorResponse("Invalid Transaction PIN", null, StatusCodes.GeneralError);
            }
            var salt = CryptoServices.CreateRandomSalt();
            util = new CryptoServices(model.NewTrxPin, salt);
            var newHash = util.ComputeSaltedHash();
            user.TransactionPIN = newHash;
            user.SaltProperty = salt;
            user.PinTries = 0;
            await userManager.UpdateAsync(user);

           
            return ReturnedResponse.SuccessResponse(null, true, StatusCodes.Successful);
        }

        public async Task<ApiResponse> AddTransactionPin(AddTransactionPinModel model, string userId)
        {
            //Add transaction PIN
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User not found", null,StatusCodes.GeneralError);
            }
            var isValidPin = Utility.ValidatePin(model.TrxPin);
            if (!isValidPin)
            {
                return ReturnedResponse.ErrorResponse("Pin Number must not be same", null, StatusCodes.GeneralError);
            }
            var util = new CryptoServices(model.TrxPin, user.SaltProperty);
            user.TransactionPIN = util.ComputeSaltedHash();
            user.PinTries = 0;
            user.IsPINSet = true;
            await userManager.UpdateAsync(user);

            
            return ReturnedResponse.SuccessResponse(null, true, StatusCodes.Successful);
        }


        public async Task<ApiResponse> GetFundWalletDetails(string theUserId)
        {
            var user = await userManager.FindByIdAsync(theUserId);
            if(user == null)
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.GeneralError);

            var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == theUserId);
            if(account == null)
                return ReturnedResponse.ErrorResponse("Account not found", null, StatusCodes.GeneralError);

            var fundWallet = new FundWalletDto()
            {
               accountNumber = account.ThirdPartyAccountNumber,
               BankName = account.ThirdPartyBankName,
               FullName = user.FirstName + " " + user.LastName,
            };

            return ReturnedResponse.SuccessResponse("Fund wallet details retrieved successfully", fundWallet, StatusCodes.Successful);
        }

        public async Task<ApiResponse> GetUserAccountDetails(string theUserId)
        {
            var user = await userManager.FindByIdAsync(theUserId);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.GeneralError);

            var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == theUserId);
            if (account == null)
                return ReturnedResponse.ErrorResponse("Account not found", null, StatusCodes.GeneralError);
            
            
            var limit = 0;
            if (account.Limit == 0 )
            {

                if (account.Tier == 0)
                {
                    account.Limit = 50000;
                }

                if (account.Tier == 1)
                {
                    account.Limit = 500000;
                }

                if (account.Tier == 3)
                {
                    account.Limit = 2000000;
                }

                context.Update(account);
                await context.SaveChangesAsync();
            }

            limit = account.Limit;
            

            var accountDetails  = new AccountDetails()
            {
                accountNumber = account.ThirdPartyAccountNumber,
                BankName = account.ThirdPartyBankName,
                FullName = user.FirstName + " " + user.LastName,
                Tier = account.Tier,
                Limit = limit
                
            };

            return ReturnedResponse.SuccessResponse("Fund wallet details retrieved successfully", accountDetails, StatusCodes.Successful);
        }


        public async Task<ApiResponse> ChangePanicModePin(ChangePanicModePinModel model, string userId)
        {
            var isValidPin = Utility.ValidatePin(model.NewPanicPin);
            if (!isValidPin)
            {
                return ReturnedResponse.ErrorResponse("Pin Numbers must not be same", null, StatusCodes.GeneralError);
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("user not found", null, StatusCodes.NoRecordFound);
            }
            if (model.OldPanicPin == model.NewPanicPin)
            {
                return ReturnedResponse.ErrorResponse("Old pin cannot be the same as new pin", null, StatusCodes.GeneralError);
            }
            var util = new CryptoServices(model.OldPanicPin, user.SaltProperty);
            var hash = util.ComputeSaltedHash();
            if (user.TransactionPIN != hash)
            {
                return ReturnedResponse.ErrorResponse("Invalid Transaction PIN", null, StatusCodes.GeneralError);
            }
            var salt = CryptoServices.CreateRandomSalt();
            util = new CryptoServices(model.NewPanicPin, salt);
            var newHash = util.ComputeSaltedHash();
            user.PanicPIN = newHash;
            user.SaltProperty = salt;
            
            await userManager.UpdateAsync(user);


            return ReturnedResponse.SuccessResponse(null, true, StatusCodes.Successful);
        }

        public async Task<ApiResponse> AddPanicModePin(AddPanicModePinModel model, string userId)
        {
            //Add transaction PIN
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.GeneralError);
            }
            var isValidPin = Utility.ValidatePin(model.PanicPin);
            if (!isValidPin)
            {
                return ReturnedResponse.ErrorResponse("Panic Pin Number must not be same", null, StatusCodes.GeneralError);
            }
            var util = new CryptoServices(model.PanicPin, user.SaltProperty);

            var hash = util.ComputeSaltedHash();

            if(hash == user.TransactionPIN)
            {
                return ReturnedResponse.ErrorResponse("Panic Pin Number must not be same with transaction pin", null, StatusCodes.GeneralError);
            }

            user.PanicPIN = hash;
            user.IsPanicPINSet = true;
            await userManager.UpdateAsync(user);

            return ReturnedResponse.SuccessResponse(null, true, StatusCodes.Successful);
        }

        public async Task<ApiResponse> CheckTransactionFee(int Amount)
        {
            var resposne = await flutterWave.GetFees(Amount);
            return resposne;
        }

        public async Task<ApiResponse> validateAccountDetails(VerifyAccountUserRequestModel model)
        {
            var bank = await context.Banks.FirstOrDefaultAsync(x => x.BankName == model.bank_name);

            if(bank == null)
            {
                return ReturnedResponse.ErrorResponse("An error while fetching bank", null, StatusCodes.NoRecordFound);
            }
            var bankCode = bank.BankCode;
            var resp = await flutterWave.AccountNameVerification(new AccountNameVerificationModel()
            {
                account_bank = bankCode,
                account_number = model.account_number
            });

            if(resp.Status == Status.Successful.ToString()) 
            {
                var data = (AccountNameVerificationResponse)resp.Data;
                var verifyNameResponse = new VerifyAccountUserResponseModel()
                {
                    account_number = model.account_number,
                    bank_code = bank.BankCode,
                    bank_name = bank.BankName,
                    AccountName = data.data.account_name
                };

                return ReturnedResponse.SuccessResponse("Account Details Fetched", verifyNameResponse, StatusCodes.Successful);
            }
            return resp;
        }

        public async Task<ApiResponse> GetAllBanksWithCode ()
        {
            var banks = await context.Banks.Select(x=> new { x.BankName, x.BankCode }).OrderBy(x=> x.BankName).ToListAsync();

            return ReturnedResponse.SuccessResponse("Banks Fetched", banks, StatusCodes.Successful);
        }

        public async Task<ApiResponse> GetDataBundleByProviders(string serviceProvider)
        {
            var biller_code = "";
            if (serviceProvider == "Airtel")
            {
                biller_code = ServiceProvider.Airtel;
                var resp = await flutterWave.GetBillCategories(biller_code, "1");
                return resp;
            }
            if (serviceProvider == "GLO")
            {
                biller_code = ServiceProvider.GLO;
                var resp = await flutterWave.GetBillCategories(biller_code, "1");
                return resp;
            }
            if (serviceProvider == "MTN")
            {
                biller_code = ServiceProvider.MTN;
                var resp = await flutterWave.GetBillCategories(biller_code, "1");
                return resp;
            }
            if (serviceProvider == "NMobile")
            {
                biller_code = ServiceProvider.NMobile;
                var resp = await flutterWave.GetBillCategories(biller_code, "1");
                return resp;
            }
            else
            {
                return ReturnedResponse.ErrorResponse("Service provider doesn't exists", null, StatusCodes.GeneralError);
            }
        }
        
        public async Task<ApiResponse> PopulateBankTable()
        {

            var isPopulated = await context.Banks.ToListAsync();
            //check so as not to fill table that hs been populated (i.e this action can only be performed once per database)
            if (isPopulated.Count > 0)
            {
                return ReturnedResponse.SuccessResponse("Bank Table Already Populated", null, StatusCodes.Successful);
            }

            var resp = await flutterWave.GetAllBanks();
            var data = (BankResponseModel)resp.Data;
                     
            var thirdPartyData = data.Data;

            foreach (var item in thirdPartyData)
            {
                var bank = new Bank()
                {
                    id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    BankCode = item.Code,
                    BankName = item.Name,
                };

                context.Banks.Add(bank);
            }
            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("Bank Table Populated", null, StatusCodes.Successful);
        }


        // use this endpoint to get the last 20
        public async Task<ApiResponse> FetchUserLastTrasnasctions(string theUserId, int count = 20)
        {
            var user = await userManager.FindByIdAsync(theUserId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User doesn't exist", null, StatusCodes.NoRecordFound);
            }

            var transactions = await context.Transactions.Where(x=> x.UserId == theUserId).ToListAsync();

            //order in descending by date created
            transactions = transactions.OrderByDescending(x=> x.CreationDate).ToList();

            //take the first count numbers
            transactions  = transactions.Take(count).ToList();
            var result = getTransactionDto(transactions);

            

            return ReturnedResponse.SuccessResponse("Transactions retrieved successfully", result, StatusCodes.Successful);
        }

        private List<TransactionDto> getTransactionDto(List<Transaction> transactions)
        {
            var transactionDto = new List<TransactionDto>();
            foreach (var transaction in transactions)
            {
                var dto = map.Map<TransactionDto>(transaction);
                dto.CreatedDate = transaction.CreationDate.Value.ToString("dd/MM/yyyy");
                dto.CreatedTime = transaction.CreationDate.Value.ToString("hh:mm:ss tt");
                transactionDto.Add(dto);
            }
            return transactionDto;
        }

        public async Task<ApiResponse> GetUserDashboard(string theUserId)
        {
            var user = await userManager.FindByIdAsync(theUserId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User doesn't exist", null, StatusCodes.NoRecordFound);
            }

            var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == theUserId);
            if (account == null)
            {
                return ReturnedResponse.ErrorResponse("An error occurred User account doesn't exist", null, StatusCodes.NoRecordFound);
            }

            var apiResp = await flutterWave.GetPaymentSubaccountBalance(account.ThirdPartyReference);

            if (apiResp.Status != Status.Successful.ToString())
            {
                return apiResp;
            }

            var apiRespData = (PayoutSubaccountBalance)apiResp.Data;

            var balance = apiRespData.data.available_balance;

            var resp = await FetchUserLastTrasnasctions(theUserId);
            var transactions = (List<TransactionDto>)resp.Data;


            account.Balance = (decimal)balance;
            context.Accounts.Update(account);
            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("Dashboard details", new DashboardModel
            {
                Name = user.FirstName,
                Balance = (decimal) balance,
                ProfilePicture = user.ProficePictureId,
                Transactions = transactions,
                Lastname = user.LastName,
                Bank = account.ThirdPartyBankName,
                AccountNumber = account.ThirdPartyAccountNumber
            }, StatusCodes.Successful);
        }

        private async Task<string> GenerateWalletAccountNumber()
        {

            var initalizeAccountNumber = 1011026001;

            var accountNumberexist = await context.Accounts.OrderBy(x => x.CreationDate).LastOrDefaultAsync();

            if (accountNumberexist != null)
            {
                var accountNumber = Convert.ToInt64(accountNumberexist.AccountNumber);

                var newAccountNumber = accountNumber + 1;

                return newAccountNumber.ToString();
            }

            return initalizeAccountNumber.ToString();
        }



    }
}
