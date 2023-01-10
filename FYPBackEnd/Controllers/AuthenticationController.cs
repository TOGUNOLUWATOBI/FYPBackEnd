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

namespace FYPBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IUserService userService;
        private readonly ILogger<AuthenticationController> log;

        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> log)
        {
            this.userService = userService;
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
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null));
                }

                var resp = await userService.CreateUser(model);
                if (resp.Message == Status.Successful.ToString())
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null));
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
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null));
                }

                var resp = await userService.Login(model);
                if (resp.Message == Status.Successful.ToString())
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null));
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
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null));
                }

                var resp = await userService.DeActivateUser(email);
                if (resp.Message == Status.Successful.ToString())
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null));
            }
        }

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
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null));
                }

                var resp = await userService.GetUsers();
                if (resp.Message == Status.Successful.ToString())
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null));
            }
        }


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
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null));
                }

                var resp = await userService.GetUser(email);
                if (resp.Message == Status.Successful.ToString())
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null));
            }
        }
    }
}
