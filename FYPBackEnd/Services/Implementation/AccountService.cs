using AutoMapper;
using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.DTO;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ApiResponse> GenerateAccountNumber(ApplicationUser user, string bvn)
        {
            try
            {
                var userExist = await userManager.FindByEmailAsync(user.Email);
                if (userExist == null)
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
                if (user.Status == UserStatus.Active.ToString())
                    return ReturnedResponse.ErrorResponse("Can't initiate transaction: your account is under review, contact support", null, StatusCodes.UnverifedUser);

                //implement getting fee amount either by api call or get by api call and update it in database and use database value
                var feeAmount = (decimal)10.75;

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

                        var transactionDto = map.Map<TransactionDto>(transaction);

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
                if (user.Status == UserStatus.Active.ToString())
                    return ReturnedResponse.ErrorResponse("Can't initiate transaction: your account is under review, contact support", null, StatusCodes.UnverifedUser);

                //implement getting fee amount either by api call or get by api call and update it in database and use database value
                var feeAmount = (decimal)10.75;

                model.TrxAmount = model.Amount + feeAmount;


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
                    Amount = model.Amount,
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
                            amount = Convert.ToInt32(model.TrxAmount),
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
