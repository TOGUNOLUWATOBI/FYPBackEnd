using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Implementation
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ApplicationDbContext context;
        private readonly IOtpService otpService;
        
        public MailService(IOptions<MailSettings> mailSettings, ApplicationDbContext context, IOtpService otpService)
        {
            _mailSettings = mailSettings.Value;
            this.context = context;
            this.otpService = otpService;
        }

        public async Task<ApiResponse> SendGenericEmailAsync(MailRequestModel model)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(model.ToEmail));
            email.Subject = model.Subject;
            var builder = new BodyBuilder();
            if (model.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in model.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = model.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return ReturnedResponse.SuccessResponse("Email sent", null);
        }

        public async Task<ApiResponse> SendVerificationEmailAsync(ApplicationUser user)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(user.Email));


            
            var otpCode = await otpService.GenerateOtpCodeAsync();

            var otp = new Otp
            {
                id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                OtpCode = otpCode,
                ExpiryDate = DateTime.Now.AddMinutes(10),
                IsUsed = false,
                Purpose = OtpPurpose.UserVerification.ToString(),
                LastModifiedDate= DateTime.Now,
                Email = user.Email,
                UserId = user.Id
            };

            await context.Otps.AddAsync(otp);



            email.Subject = "User Verification";
            var builder = new BodyBuilder();
            builder.TextBody = otpCode;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("Email sent", null);
        }   
    }
}
