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
using FYPBackEnd.Data.Enums;

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
        private Mock<IAccountService> accountService;
        private Mock<IUVerify> uverify;
        private Mock<IGoogleDrive> googleDrive;
        //private Mock<>

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
            accountService = new Mock<IAccountService>();
            uverify = new Mock<IUVerify>();
            googleDrive = new Mock<IGoogleDrive>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile()); //your automapperprofile 
            });
            var mapper = mockMapper.CreateMapper();

            _appSettings = new Mock<IOptions<AppSettings>>();

            var app = new AppSettings()
            {
                JwtLifespan = 5,
                JwtSecret  = "password1234567890",
                ValidAudience = "User",
                ValidIssuer = "Admin"
            };
            _appSettings.Setup(a => a.Value).Returns(app);

            
            ApiResponse resp = ReturnedResponse.SuccessResponse(null,null,null);
            mailService.Setup(m => m.SendVerificationEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(resp);
            

            _service = new UserService(_userManager.Object, _context.Object, _signInManager.Object, mapper, mailService.Object, _appSettings.Object,accountService.Object, uverify.Object, googleDrive.Object,otpService.Object);
        }

        [Test]
        public async Task SignUp()
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


        // add a test case for login
        [Test]
        public async Task Login()
        {
            LoginRequestModel model = new LoginRequestModel()
            {
                EmailAddress = "togunoluwatobi@gmail.com",
                Password = "String@3105"
            };
            _userManager.Setup(x=> x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            _signInManager.Setup(x=> x.PasswordSignInAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>(),false,true)). ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);


            ////mock appsettings
            //_appSettings.Setup(m => m.Value.JwtSecret).Returns(It.IsAny<string>());
            //_appSettings.Setup(m => m.Value.ValidAudience).Returns(It.IsAny<string>());
            //_appSettings.Setup(m => m.Value.ValidIssuer).Returns(It.IsAny<string>());
            //_appSettings.Setup(m => m.Value.JwtLifespan).Returns(It.IsAny<int>());


            var response = await _service.Login(model);
            Assert.AreEqual(response.Status, "Successful");
        }

        //add a testcase for activate user
        [Test]
        public async Task PassTestActivateUser()
        {
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser()
            {
                Status = UserStatus.Inactive.ToString(),
            });

            var response = await _service.ActivateUser(It.IsAny<string>());
            Assert.AreEqual(response.Status, "Successful");
        }



        [Test]
        public async Task FailTestActivateUserBlacklisted()
        {
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser()
            {
                Status = UserStatus.Blacklisted.ToString(),
            });

            var response = await _service.ActivateUser(It.IsAny<string>());
            Assert.AreEqual(response.Status, "UnSuccessful");
        }





    }
}