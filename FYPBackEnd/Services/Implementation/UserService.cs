﻿using AutoMapper;
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

namespace FYPBackEnd.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper map;
        private readonly IMailService mailService;
        private readonly AppSettings _appSettings;


        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IMapper map, IMailService mailService, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            this.context = context;
            _signInManager = signInManager;
            this.map = map;
            this.mailService = mailService;
            _appSettings = appSettings.Value;
        }

        public async Task<ApiResponse> ActivateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User not found", null);
            if (user.Status == UserStatus.Blacklisted.ToString())
                return ReturnedResponse.ErrorResponse("Can't activate user as user is blacklisted", null);
            user.Status = UserStatus.Active.ToString();
            await context.SaveChangesAsync();

            return ReturnedResponse.ErrorResponse("User successfully activated", null);
        }

        public async Task<ApiResponse> CreateUser(SignUpRequestModel model)
        {
            var isUserExist = await _userManager.FindByEmailAsync(model.Email);

            if (isUserExist != null)
                return ReturnedResponse.ErrorResponse("User with this email already exists", null);

            var user = new ApplicationUser();
            user.Email = model.Email;
            user.UserName = model.Email;
            //user.Password = _userManager.PasswordHasher.HashPassword(user,model.Password);
            user.FirstName = model.Firstname;
            user.LastName = model.Lastname;
            user.PhoneNumber = Core.Utility.FormatPhoneNumber(model.PhoneNumber);
            user.Address = model.Address;
            user.Lga = model.LGA;
            user.State = model.State;
            user.Status = UserStatus.Inactive.ToString();
            user.Gender = model.Gender;
            //todo: send otp for user to activate/confirm email/phonenumber
            _ = await mailService.SendVerificationEmailAsync(user, OtpPurpose.UserVerification.ToString());

            var x = await _userManager.CreateAsync(user,model.Password);
            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User successfuly registered", userDto);
        }

        public async Task<ApiResponse> DeActivateUser(string email)
        {
            // change the implementation do not delete user accoounts easily just make user inactive
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User not found", null);
            if (user.Status == UserStatus.Blacklisted.ToString())
                return ReturnedResponse.ErrorResponse("Can't deactivate user as user is blacklisted", null);
            user.Status = UserStatus.Inactive.ToString();
            context.Update(user);
            await context.SaveChangesAsync();

            return ReturnedResponse.ErrorResponse("User successfully deactivated", null);
        }

        

        public async Task<ApiResponse> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User with that email couldn't be found", null);

            //todo: change user model to a user dto model to avoid exposing hashed password
            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User found", userDto);
        }

        public async Task<ApiResponse> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            return ReturnedResponse.SuccessResponse("All users", users);
        }

        public async Task<ApiResponse> Login(LoginRequestModel model)
        {
            
            if (string.IsNullOrEmpty(model.EmailAddress))
                return ReturnedResponse.ErrorResponse("User Email can't be null or empty", null);
            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user != null)
            {

                var isValid = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);    //_userManager.PasswordHasher.VerifyHashedPassword(user, user.Password,model.Password);

                if (isValid.Succeeded)
                {
                    //todo: check if the user is verifed, if not verified resend otp
                    //todo: generate jwt to send to the front end


                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSecret));

                    var token = new JwtSecurityToken(
                        issuer: _appSettings.ValidIssuer,
                        audience: _appSettings.ValidAudience,
                        expires: DateTime.Now.AddHours(_appSettings.JwtLifespan), 
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    var bearerToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var loginResponseModel = new LoginResponseModel
                    {
                        AccessToken = bearerToken,
                        TokenType = "Bearer",
                        Email = model.EmailAddress,
                        ExpiresIn = DateTime.Now.AddHours(_appSettings.JwtLifespan)
                    };
                   
                    //todo: create login response model and send to get jwt and extra stuff
                    return ReturnedResponse.SuccessResponse("User Successfully logged in", loginResponseModel);
                }

            }

            return ReturnedResponse.ErrorResponse("Invalid Email/Password", null);
        }

        public Task<ApiResponse> UpdateUser()
        {
            throw new System.NotImplementedException();
        }
    }
}
