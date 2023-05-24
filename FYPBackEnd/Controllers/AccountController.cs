using FYPBackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FYPBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IGoogleDrive googleDrive;
        public static IWebHostEnvironment _environment;
        private readonly ILogger<AuthenticationController> log;

        public AccountController(IAccountService accountService, IGoogleDrive googleDrive, ILogger<AuthenticationController> log, IWebHostEnvironment environment)
        {
            this.accountService = accountService;
            this.googleDrive = googleDrive;
            this.log = log;
            _environment = environment;
        }



    }
}
