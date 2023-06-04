using FYPBackEnd.Data;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.FlutterWave;
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
    public class WebhookController : Controller
    {
        private readonly IWebhookService webhook;
        public static IWebHostEnvironment _environment;
        private readonly ApplicationDbContext context;
        private readonly ILogger<WebhookController> log;

        public WebhookController(IWebhookService webhook,  ILogger<WebhookController> log, IWebHostEnvironment environment)
        {
            this.webhook = webhook;
            this.log = log;
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/flutterwave")]
        public async Task<IActionResult> FWHandleWebhook([FromBody] WebhookRequest request)
        {
            try
            {
                context.FW.Add(new Data.Entities.FlutterwaveWebhook()
                {
                    id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    webhook = request.ToString(),
                });
                await context.SaveChangesAsync();
                var theSecretHash = Request.Headers["verif-hash"];
                var resp = await webhook.FWHandleWebhook(request, theSecretHash);
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
                log.LogError(ex, "Error occured while processing flutterwave webhook");
                return BadRequest(ReturnedResponse.ErrorResponse($"an error occured while resolving the webhook: {ex.Message}",null,StatusCodes.ExceptionError));
            }
        }
    }
}
