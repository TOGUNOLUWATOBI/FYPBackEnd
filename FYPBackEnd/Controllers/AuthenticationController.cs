using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.Constants;
using System.IO;
using Microsoft.AspNetCore.Http;
using StatusCodes = FYPBackEnd.Data.Constants.StatusCodes;

namespace FYPBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IGoogleDrive googleDrive;
        private readonly ILogger<AuthenticationController> log;

        public AuthenticationController(IUserService userService,IGoogleDrive googleDrive, ILogger<AuthenticationController> log)
        {
            this.userService = userService;
            this.googleDrive = googleDrive;
            this.log = log;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.CreateUser(model);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat("Error occured in the SignUp", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.GeneralError));
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.Login(model);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat("Error occured in the Login", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.GeneralError));
            }
        }

        
        [HttpPost]
        [Route("api/v1/DeactivateUser")]
        public async Task<IActionResult> DeactivateUser(string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.DeActivateUser(email);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in the Deactivation of user with email: {email}", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.GeneralError));
            }
        }

        //[Authorize]
        [HttpGet]
        [Route("api/v1/Users")]
        
        
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.GetUsers();
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in retrieving all users", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.GeneralError));
            }
        }

        //[Authorize]
        [HttpGet]
        [Route("api/v1/GetUser")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.GetUser(email);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in the getting user ", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.GeneralError));
            }
        }

        [HttpPost]
        [Route("api/v1/VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(string otpCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.VerifyOtp(otpCode);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in verifying otp ", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error occured in verifying otp", null, StatusCodes.GeneralError));
            }
        }

        [HttpPost]
        [Route("api/v1/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.ResetPassword(model);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in resetting password", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error occured in resseting password", null, StatusCodes.GeneralError));
            }
        }


        //todo: change this to the right controller
        //todo: decrypt jwt passed soa s to know the user name/id topass to save the picture


        [HttpPost]
        [Route("api/v1/UploadPicture")]
        public async Task<IActionResult> UploadPicture([FromForm] UploadImageRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await googleDrive.UploadFileWithMetaData(model.file);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in uploading picture", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error occured in uploading picutre: {errMessage}", null, StatusCodes.GeneralError));
            }
        }

        [HttpPost]
        [Route("api/v1/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await userService.ForgotPasswordRequest(email);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in resetting password", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error occured in resseting password", null, StatusCodes.GeneralError));
            }
        }
    }
}
