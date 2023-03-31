﻿using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using System.Globalization;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse> CreateUser(SignUpRequestModel model);
        Task<ApiResponse> UpdateUser();
        Task<ApiResponse> DeActivateUser(string email);
        Task<ApiResponse> ActivateUser(string token);
        Task<ApiResponse> GetUser(string email);
        Task<ApiResponse> GetUsers();
        Task<ApiResponse> Login(LoginRequestModel model);

        Task<ApiResponse> ForgotPasswordRequest(string email);
        Task<ApiResponse> ResetPassword(ChangePasswordRequestModel model);
        Task<ApiResponse> VerifyOtp(string otpCode);

    }
}
