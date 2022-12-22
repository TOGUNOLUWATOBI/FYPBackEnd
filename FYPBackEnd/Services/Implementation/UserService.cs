using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext context;


        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            this.context = context;
        }

        public Task<ApiResponse> ActivateUser(string token)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApiResponse> CreateUser(SignUpRequestModel model)
        {
            var isUserExist = await _userManager.FindByEmailAsync(model.Email);

            if (isUserExist != null)
                return ReturnedResponse.ErrorResponse("User with this email already exists", null);

            var user = new ApplicationUser();
            user.Email = model.Email;
            user.Password = _userManager.PasswordHasher.HashPassword(user,model.Password);
            user.FirstName = model.Firstname;
            user.LastName = model.Lastname;
            user.PhoneNumber = Utility.FormatPhoneNumber( model.PhoneNumber);
            user.Address = model.Address;
            user.Lga = model.LGA;
            user.State = model.State;
            user.Status = UserStatus.Inactive.ToString();
            user.Gender = model.Gender;

            return ReturnedResponse.SuccessResponse("User successfuly registered", user);
        }

        public async Task<ApiResponse> DeActivateUser(string email)
        {
            // change the implementation do not delete user accoounts easily just make user inactive

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User not found", null);
            user.Status = UserStatus.Inactive.ToString();
            await context.SaveChangesAsync();

            return ReturnedResponse.ErrorResponse("User successfully deactivated", null);

        }

        

        public async Task<ApiResponse> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User with that email couldn't be found", null);

            //todo: change user model to a user dto model to avoid exposing hashed password
            return ReturnedResponse.SuccessResponse("User found", user);
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

            if (user == null)
                return ReturnedResponse.ErrorResponse("Invalid Email/Password", null);
            var verifiedCorrectPassword = _userManager.PasswordHasher.VerifyHashedPassword(user, user.Password,model.Password);

            if (verifiedCorrectPassword.ToString().Equals("Success"))
                return ReturnedResponse.SuccessResponse("User Successfully logged in", null);

            return ReturnedResponse.ErrorResponse("Invalid Email/Password", null);
        }

        public Task<ApiResponse> UpdateUser()
        {
            throw new System.NotImplementedException();
        }
    }
}
