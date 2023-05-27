using AutoMapper;
using epAgentAuthentication.Services;
using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.DTO;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.Models.ResponseModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FYPBackEnd.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFlutterWave flutterWave;
        private readonly IMapper map;
        public AccountService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFlutterWave flutterWave, IMapper map)
        {
            this.context = context;
            this.userManager = userManager;
            this.flutterWave = flutterWave;
            this.map = map;
        }

        public async Task<ApiResponse> GenerateAccountNumber(string userId)
        {
            try
            {
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

                        if (accountRef != resp.data.account_reference)
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
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                // check if user is blacklisted or inactive
                if (user.Status != UserStatus.Active.ToString())
                    return ReturnedResponse.ErrorResponse("Can't initiate transaction: your account is under review, contact support", null, StatusCodes.UnverifedUser);

                //todo: check whether the transaction pin is correct or not and also check if it panic mode

                //todo: implement getting fee amount either by api call or get by api call and update it in database and use database value
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
                    Beneficiary = string.Concat(model.BeneficiaryName, " | ", model.BeneficiaryAccountNumber, " | ", model.BeneficiaryBank, $" | {user.PhoneNumber}"),
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


                // call flutterwave to process the transaction
                var flutterTransfer = await flutterWave.InitiateTransfer(new InitiateTransferRequestModel()
                {
                    Debit_currency = "NGN",
                    Debit_subaccount = account.ThirdPartyReference,
                    Account_bank = model.BeneficiaryBankCode,
                    Account_number = model.BeneficiaryAccountNumber,
                    Amount = model.Amount,
                    Currency = "NGN",
                    Narration = model.Description,
                    Reference = reference
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

                        var transactionDto = map.Map<TransferDto>(transaction);

                        transactionDto.BeneficiaryName = model.BeneficiaryName;
                        transactionDto.BeneficiaryBank = model.BeneficiaryBank;

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
                    Beneficiary = string.Concat(model.Customer),
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

                //updated logic
                // perform transfer to flutterwave pool account and then after successful transfer to the right user
                //todo: ask ekundayo if i should log this as a transaction time no dey sha

                var flutterTransfer = await flutterWave.InitiateTransfer(new InitiateTransferRequestModel()
                {
                    Debit_currency = "NGN",
                    Debit_subaccount = account.ThirdPartyReference,
                    Account_bank = "035",
                    Account_number = "8540683210",
                    //check if decimal can be sent on the api and use to confirm
                    Amount = Convert.ToInt32(model.TransferAmount),
                    Currency = "NGN",
                    Narration = $"Internal Transfer for AirtimeData for : {model.Customer}",
                    Reference = reference
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

                                var transactionDto = map.Map<TransferDto>(transaction);

                                transactionDto.PhoneNumber = model.Customer;

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
                var verifyNameResponse = new VerifyAccountUserResponseModel()
                {
                    account_number = model.account_number,
                    bank_code = bank.BankCode,
                    bank_name = bank.BankName,
                };

                return ReturnedResponse.SuccessResponse("Account Details Fetched", verifyNameResponse, StatusCodes.Successful);
            }
            return resp;
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

        //public async Task<ApiResponse> FetchUserLastTrasnasction(string theUserId)
        //{
        //    var user = await userManager.FindByIdAsync(theUserId);
        //    if (user == null)
        //    {
        //        return ReturnedResponse.ErrorResponse("User doesn't exist", null, StatusCodes.NoRecordFound);
        //    }
        //}

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
