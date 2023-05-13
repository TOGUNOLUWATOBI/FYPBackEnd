using AutoMapper;
using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.DTO;
using FYPBackEnd.Data.Models.FlutterWave;
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
                    User = user,
                    Status = AccountStatus.Active.ToString(),
                    AccountName = user.FirstName + " " + user.LastName,
                    AccountNumber = accountNumber,
                    Balance = 0,
                };



                var flutterWaveAccount = await flutterWave.CreateVirtualStaticAccount(new CreateVIrtualRequestModel()
                {
                    Bvn = bvn,
                    Email = user.Email,
                    Firstname = user.FirstName,
                    Lastname = user.LastName,
                    is_permanent = true,
                    tx_ref = user.Id,
                    Phonenumber = user.PhoneNumber,
                    Narration = account.AccountName
                });

                if (flutterWaveAccount != null)
                {
                    if (flutterWaveAccount.Status == Status.Successful.ToString())
                    {
                        var resp = (CreateVIrtualResponseModel)flutterWaveAccount.Data;

                        account.ThirdPartyAccountNumber = resp.Data.account_number;
                        account.ThirdPartyBankName = resp.Data.bank_name;
                        account.ThirdPartyReference = resp.Data.order_ref;

                        await context.AddAsync(account);
                        await context.SaveChangesAsync();

                        var accountDto = map.Map<AccountDto>(account);

                        return ReturnedResponse.SuccessResponse("Account Successfully created", accountDto, StatusCodes.Successful);
                    }
                }

                return ReturnedResponse.ErrorResponse("An error occured account couldn't be created",flutterWaveAccount.Data, StatusCodes.GeneralError);
            }
            catch(Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message ?? ex.InnerException.ToString(), null, StatusCodes.ExceptionError);
            }
        }

        //public async Task<ApiResponse> InitiateTransfer ()
        //{

        //}


        private  async Task<string> GenerateWalletAccountNumber()
        {
            
            var initalizeAccountNumber = 1011026001;
            
            var accountNumberexist = await context.Accounts.OrderBy(x=> x.CreationDate).LastOrDefaultAsync();
            
            if(accountNumberexist != null)
            {
                var accountNumber = Convert.ToInt64(accountNumberexist.AccountNumber);

                var newAccountNumber = accountNumber + 1;

                return newAccountNumber.ToString();
            }

            return initalizeAccountNumber.ToString();
        }
    }
}
