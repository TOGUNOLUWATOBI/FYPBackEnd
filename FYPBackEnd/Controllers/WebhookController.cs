using FYPBackEnd.Data;
using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Implementation;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : Controller
    {
        private readonly IWebhookService webhook;
        private readonly FlutterWaveSettings flutterWaveSettings;
        public static IWebHostEnvironment _environment;
        private readonly ApplicationDbContext context;
        private readonly ILogger<WebhookController> log;

        public WebhookController(IWebhookService webhook, ILogger<WebhookController> log, IWebHostEnvironment environment, ApplicationDbContext context, IOptions<FlutterWaveSettings> flutterWaveSettings)
        {
            this.webhook = webhook;
            this.log = log;
            _environment = environment;
            this.flutterWaveSettings = flutterWaveSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/flutterwave")]
        public async Task<IActionResult> FWHandleWebhook([FromBody] WebhookRequest request)
        {
            try
            {
                var fw = new FlutterwaveWebhook() {id = Guid.NewGuid(), webhook = request.ToString(), CreationDate = DateTime.Now , LastModifiedDate = DateTime.Now};
                context.Add(fw);

                await context.SaveChangesAsync();
                var requestHeaders = new Dictionary<string, string>();
                foreach (var header in Request.Headers)
                {
                    requestHeaders.Add(header.Key, header.Value);
                }

                var theSecretHash = Request.Headers["verif-hash"].ToString();
               

                //check to make sure that the webhook is from fluttereave using the secret hash
                var util = new CryptoServices(flutterWaveSettings.SecretHash, flutterWaveSettings.SaltProperty);
                var util2 = new CryptoServices(theSecretHash, flutterWaveSettings.SaltProperty);

                var hash = util.ComputeSaltedHash();
                var hash2 = util2.ComputeSaltedHash();



                if (hash != hash2)
                {
                    return BadRequest( ReturnedResponse.ErrorResponse("The webhook is not from the right source ", request, StatusCodes.GeneralError));
                }

                var resp = await webhook.FWHandleWebhook(request);
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
