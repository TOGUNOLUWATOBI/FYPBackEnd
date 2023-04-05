using AutoMapper;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Services.Implementation;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.IO;
using FYPBackEnd.Data.Models.RequestModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Data.Models.ViewModel;
using FYPBackEnd.Core;

namespace FYPBackendUnitTest
{
    public class Tests
    {
        private Mock<UserManager<ApplicationUser>> _userManager;
        private Mock<ApplicationDbContext> _context;
        private Mock< SignInManager<ApplicationUser>> _signInManager;
        private Mock<IMapper> map;
        private Mock<IMailService> mailService;
        private Mock<IOptions<AppSettings>> _appSettings;
        private Mock<IOtpService> otpService;

        private UserService _service;


        //[OneTimeSetUp]
        //public void GlobalPrepare()
        //{
        //    var configuration = new ConfigurationBuilder()
        //       .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", false)
        //    .Build();

        //    _appSettings = Options.Create(configuration.GetSection(nameof(AppSettings))).Value;
        //}

        [SetUp]
        public void Setup()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _context = new Mock<ApplicationDbContext>();
            _signInManager = new Mock<SignInManager<ApplicationUser>>(_userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),null,null,null,null);
            map = new Mock<IMapper>();
            otpService = new Mock<IOtpService>();
            mailService = new Mock<IMailService>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile()); //your automapperprofile 
            });
            var mapper = mockMapper.CreateMapper();

            _appSettings = new Mock<IOptions<AppSettings>>();
            ApiResponse resp = ReturnedResponse.SuccessResponse(null,null,null);
            mailService.Setup(m => m.SendVerificationEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(resp);

            _service = new UserService(_userManager.Object, _context.Object, _signInManager.Object, mapper, mailService.Object, _appSettings.Object);
        }

        [Test]
        public async Task Test1()
        {
            
            SignUpRequestModel model = new SignUpRequestModel()
            {
                Email = "togunoluwatobi@gmail.com",
                State = "Lagos",
                Password = "String@3105",
                Address = "18, Morocco Road",
                Lastname = "Togun",
                Firstname = "Oluwatobi",
                Gender = "Male",
                LGA = "Shomolu",
                PhoneNumber = "09018866641"
            };

            

            

            var response = await _service.CreateUser(model);
            Assert.AreEqual(response.Status, "Successful");
        }
    }
}