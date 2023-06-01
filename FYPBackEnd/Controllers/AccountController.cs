using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Implementation;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FYPBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IGoogleDrive googleDrive;
        public static IWebHostEnvironment _environment;
        private readonly ILogger<AccountController> log;

        public AccountController(IAccountService accountService, IGoogleDrive googleDrive, ILogger<AccountController> log, IWebHostEnvironment environment)
        {
            this.accountService = accountService;
            this.googleDrive = googleDrive;
            this.log = log;
            _environment = environment;
        }

        [HttpPost]
        [Route("api/v1/AddPanicMode")]


        public async Task<IActionResult> AddPanicMode(AddPanicModePinModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.AddPanicModePin(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Adding Panic Mode for the user", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [HttpPost]
        [Route("api/v1/AddTransactionPin")]


        public async Task<IActionResult> AddTransactionPin(AddTransactionPinModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.AddTransactionPin(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Adding Transaction Pin", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        [HttpPost]
        [Route("api/v1/InitiateTransfer")]


        public async Task<IActionResult> InitiateTransfer(TransferRequestModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.InitiateTransfer(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Initiating transfer", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        [HttpPost]
        [Route("api/v1/BuyAiritme")]


        public async Task<IActionResult> BuyAiritmeData(BuyAirtimeRequestModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.BuyAirtimeData(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Buying Airtime/Data", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [HttpPost]
        [Route("api/v1/CheckTransactionPanicPin")]


        public async Task<IActionResult> CheckTransactionPanicPin(CheckTransactionPinModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.CheckTransactionPanicPin(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Checking Pin", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [HttpPost]
        [Route("api/v1/ChangeTransactionPin")]


        public async Task<IActionResult> ChangeTransactionPin(ChangeTransactionPinModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.ChangeTransactionPin(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Changing Transaction pin", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        [HttpPost]
        [Route("api/v1/ChangePanicModePin")]


        public async Task<IActionResult> ChangePanicModePin(ChangePanicModePinModel model)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.ChangePanicModePin(model, theUserId);
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
                log.LogInformation(string.Concat($"Error occured in Changing Panic pin", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        [HttpGet]
        [Route("api/v1/GetAmountFees")]


        public async Task<IActionResult> GetAmountFees(int amount)
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
                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.CheckTransactionFee(amount);
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
                log.LogInformation(string.Concat($"Error occured in getting amount fees", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }
       


        [HttpGet]
        [Route("api/v1/VerifyAccountDetaails")]
        public async Task<IActionResult> VerifyAccountDetaails(VerifyAccountUserRequestModel model)
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
                
                var resp = await accountService.validateAccountDetails(model);
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
                log.LogInformation(string.Concat($"Error occured in verifying account details", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [HttpGet]
        [Route("api/v1/banks")]
        public async Task<IActionResult> GetAllBanks()
        {
            try
            {
                
                var resp = await accountService.GetAllBanksWithCode();
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
                log.LogInformation(string.Concat($"Error occured in getting banks", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error has occured : {ex.Message}", null, StatusCodes.ExceptionError));
            }
        }



        [HttpGet]
        [Route("api/v1/GetDataBundles")]
        public async Task<IActionResult> GetDataBundles(string serviceProvier)
        {
            try
            {

                var resp = await accountService.GetDataBundleByProviders(serviceProvier);
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
                log.LogInformation(string.Concat($"Error occured in Getting Data Bundles", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error has occured : {ex.Message}", null, StatusCodes.ExceptionError));
            }
        }


        [HttpPost]
        [Route("api/v1/PopulateBankTable")]
        public async Task<IActionResult> PopulateBankTable()
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

                var resp = await accountService.PopulateBankTable();
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
                log.LogInformation(string.Concat($"Error occured in populating bank table", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [HttpPost]
        [Route("api/v1/GenerateAccountNumber")]
        public async Task<IActionResult> GenerateAccountNumber()
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

                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity);
                var resp = await accountService.GenerateAccountNumber(theUserId);
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
                log.LogInformation(string.Concat($"Error occured in populating bank table", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        private string GetUserId(ClaimsIdentity identity)
        {
            
            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;

            // Gets name from claims. Generally it's an email address.
            return claim.Where(x => x.Type == "userId")
                .FirstOrDefault().Value;
        }
    }
}
