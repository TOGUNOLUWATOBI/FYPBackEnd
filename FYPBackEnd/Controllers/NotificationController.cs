using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.Notification;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FYPBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationService notificationService;
        public static IWebHostEnvironment _environment;
        private readonly ILogger<NotificationController> log;
        public NotificationController(INotificationService notificationService, ILogger<NotificationController> log, IWebHostEnvironment environment)
        {
            this.notificationService = notificationService;
            this.log = log;
            _environment = environment;
        }

        [Route("api/v1/send")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendNotification(NotificationModel notificationModel)
        {
            try
            {
                var resp = await notificationService.SendNotification(notificationModel);
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
                log.LogInformation(string.Concat("Error occured in sending notifcation", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error has occured {ex.Message}", null, StatusCodes.ExceptionError));
            }
        }
    }
}
