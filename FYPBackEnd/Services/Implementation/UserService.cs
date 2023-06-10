using AutoMapper;
using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.Models.ViewModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using FYPBackEnd.Data.Models.ResponseModel;
using FYPBackEnd.Settings;
using FYPBackEnd.Data.Constants;
using System.Collections.Generic;
using System.Security.Claims;

namespace FYPBackEnd.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper map;
        private readonly IMailService mailService;
        private readonly IAccountService accountService;
        private readonly AppSettings _appSettings;


        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IMapper map, IMailService mailService, IOptions<AppSettings> appSettings, IAccountService accountService)
        {
            _userManager = userManager;
            this.context = context;
            _signInManager = signInManager;
            this.map = map;
            this.mailService = mailService;
            _appSettings = appSettings.Value;
            this.accountService = accountService;
        }

        public async Task<ApiResponse> ActivateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.NoRecordFound);

            }
            if (user.Status == UserStatus.Blacklisted.ToString())
            {
                return ReturnedResponse.ErrorResponse("Can't activate user as user is blacklisted", null, StatusCodes.BlacklistedUser);
            }

            user.Status = UserStatus.Active.ToString();
            user.LastModifiedDate = DateTime.Now;
            user.EmailConfirmed = true;
            context.Update(user);
            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("User successfully activated", null, StatusCodes.Successful);

        }

        public async Task<ApiResponse> CreateUser(SignUpRequestModel model)
        {
            var isUserExist = await _userManager.FindByEmailAsync(model.Email);

            if (isUserExist != null)
                return ReturnedResponse.ErrorResponse("User with this email already exists", null, StatusCodes.RecordExist);

            var resp = Core.Utility.ValidatePassword(model.Password);
            if (resp.Status == Status.UnSuccessful.ToString())
            {
                return resp;
            }

            var user = new ApplicationUser();
            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.Firstname;
            user.LastName = model.Lastname;
            user.PhoneNumber = Core.Utility.FormatPhoneNumber(model.PhoneNumber);
            user.Address = model.Address;
            user.SaltProperty = CryptoServices.CreateRandomSalt();
            user.IsPINSet = false;
            user.PinTries = 0;
            user.Lga = model.LGA;
            user.State = model.State;
            user.Status = UserStatus.Inactive.ToString();
            user.Gender = model.Gender;
            user.IsKYCComplete = false;
            user.LastModifiedDate = DateTime.Now;
            user.CreationDate = DateTime.Now;

            resp = await mailService.SendVerificationEmailAsync(user, OtpPurpose.UserVerification.ToString());

            if (resp.Status == Status.UnSuccessful.ToString())
                return resp;


            var x = await _userManager.CreateAsync(user, model.Password);
            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User successfuly registered", userDto, StatusCodes.Successful);
        }

        public async Task<ApiResponse> DeActivateUser(string email)
        {
            // change the implementation do not delete user accoounts easily just make user inactive
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.NoRecordFound);
            if (user.Status == UserStatus.Blacklisted.ToString())
                return ReturnedResponse.ErrorResponse("Can't deactivate user as user is blacklisted", null, StatusCodes.BlacklistedUser);
            user.Status = UserStatus.Inactive.ToString();
            user.LastModifiedDate = DateTime.Now;
            context.Update(user);
            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("User successfully deactivated", null, StatusCodes.Successful);
        }



        public async Task<ApiResponse> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User with that email couldn't be found", null, StatusCodes.NoRecordFound);


            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User found", userDto, StatusCodes.Successful);
        }

        public async Task<ApiResponse> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            return ReturnedResponse.SuccessResponse("All users", users, StatusCodes.Successful);
        }

        public async Task<ApiResponse> Login(LoginRequestModel model)
        {

            if (string.IsNullOrEmpty(model.EmailAddress))
                return ReturnedResponse.ErrorResponse("User Email can't be null or empty", null, StatusCodes.ModelError);
            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user != null)
            {

                var isValid = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);    //_userManager.PasswordHasher.VerifyHashedPassword(user, user.Password,model.Password);

                if (isValid.Succeeded)
                {


                    if (user.Status == UserStatus.Inactive.ToString() && user.EmailConfirmed == false)
                    {
                        _ = await mailService.SendVerificationEmailAsync(user, OtpPurpose.UserVerification.ToString());
                        return ReturnedResponse.SuccessResponse("Your account has not been verified, check your email to verify your account", null, StatusCodes.UnverifedUser);

                    }

                    var authClaims = new List<Claim>
                    {
                      new Claim("userId", user.Id)
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSecret));

                    var token = new JwtSecurityToken(
                        issuer: _appSettings.ValidIssuer,
                        audience: _appSettings.ValidAudience,
                        expires: DateTime.Now.AddHours(_appSettings.JwtLifespan),
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                        claims: authClaims
                        );

                    var bearerToken = new JwtSecurityTokenHandler().WriteToken(token);

                    

                    var loginResponseModel = new LoginResponseModel
                    {
                        AccessToken = bearerToken,
                        TokenType = "Bearer",
                        Email = model.EmailAddress,
                        ExpiresIn = DateTime.Now.AddHours(_appSettings.JwtLifespan),
                    };


                    return ReturnedResponse.SuccessResponse("User Successfully logged in", loginResponseModel, StatusCodes.Successful);

                }

            }

            return ReturnedResponse.ErrorResponse("Invalid Email/Password", null, StatusCodes.GeneralError);

        }

        public async Task<ApiResponse> ForgotPasswordRequest(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User with this email not found", null, StatusCodes.NoRecordFound);

            }
            else
            {
                var resp = await mailService.SendForgotPasswordEmailAsync(user, OtpPurpose.PasswordReset.ToString());
                if (resp.Status == Status.Successful.ToString())
                    return ReturnedResponse.SuccessResponse("Your email has been sent to you to reset your password", null, StatusCodes.Successful);
                else
                    return ReturnedResponse.ErrorResponse("Couldn't send email to reset password", null, StatusCodes.GeneralError);
            }
        }


        
        public async Task<ApiResponse> ResetPassword(ChangePasswordRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("No Recond found", null, StatusCodes.NoRecordFound);
            }

            var resp = Core.Utility.ValidatePassword(model.Password);
            if (resp.Status == Status.UnSuccessful.ToString())
            {
                return resp;
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.Password);

            return ReturnedResponse.SuccessResponse("Password has been reset", null, StatusCodes.Successful);
        }

        public async Task<ApiResponse> VerifyOtp(string otpCode)
        {
            var otp = await context.Otps.FirstOrDefaultAsync(x => x.OtpCode == otpCode);

            if (otp == null)
            {
                return ReturnedResponse.ErrorResponse("Invalid otp, input correct otp code", null, StatusCodes.GeneralError);

            }

            if (otp.ExpiryDate < DateTime.Now)
            {
                return ReturnedResponse.ErrorResponse("Otp has expired", null, StatusCodes.GeneralError);

            }
            if (otp.Purpose == OtpPurpose.UserVerification.ToString())
            {
                var user = await _userManager.FindByEmailAsync(otp.Email);
                if (user == null)
                {
                    return ReturnedResponse.ErrorResponse("User account couldn't be verified", null, StatusCodes.NoRecordFound);
                }

                await accountService.GenerateAccountNumber(user.Id);

                return await ActivateUser(user.Email);
            }

            else if (otp.Purpose == OtpPurpose.PasswordReset.ToString())
            {
                var user = await _userManager.FindByEmailAsync(otp.Email);
                if (user == null)
                {
                    return ReturnedResponse.ErrorResponse("User account couldn't be verified", null, StatusCodes.NoRecordFound);
                }

                return ReturnedResponse.SuccessResponse("User password reseted", null, StatusCodes.Successful);


            }

            else
            {
                return ReturnedResponse.ErrorResponse("An error occured while verifying otp", null, StatusCodes.GeneralError);
            }
        }

        public Task<ApiResponse> UpdateUser()
        {
            throw new System.NotImplementedException();
        }
    }
}
