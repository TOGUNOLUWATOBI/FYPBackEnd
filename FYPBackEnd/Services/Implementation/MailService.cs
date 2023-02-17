using FYPBackEnd.Core;
using FYPBackEnd.Data;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
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
            MailAddress to = new MailAddress(model.ToEmail);
            MailAddress from = new MailAddress(_mailSettings.Mail,_mailSettings.DisplayName);
            MailMessage message = new MailMessage(from, to);
            message.Subject = model.Subject;
            message.Body = model.Body;
            SmtpClient client = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password),
                EnableSsl = true
                // specify whether your host accepts SSL connections
            };
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            if (model.Attachments != null)
            {
                Stream fileBytes;
                foreach (var file in model.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms;
                        }
                        var attachment = new Attachment(fileBytes, file.FileName,file.ContentType);
                        message.Attachments.Add(attachment);
                    }
                }
            }
            // code in brackets above needed if authentication required
            try
            {
                client.Send(message);
                return ReturnedResponse.SuccessResponse("Email sent", null);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                return ReturnedResponse.ErrorResponse("Couldn't send email", null);
            }
            
        }

        public async Task<ApiResponse> SendVerificationEmailAsync(ApplicationUser user)
        {
            

            
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

            MailAddress to = new MailAddress(user.Email);
            MailAddress from = new MailAddress(_mailSettings.Mail,_mailSettings.DisplayName);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "User Verification";
            message.Body =otpCode;
            SmtpClient client = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                
                EnableSsl = true,
                // specify whether your host accepts SSL connections
            };
            
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            
            // code in brackets above needed if authentication required
            try
            {
                client.Send(message);
                return ReturnedResponse.SuccessResponse("Email sent", null);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                return ReturnedResponse.ErrorResponse("Couldn't send email", null);
            }

            //var email = new MimeMessage();
            //email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            //email.To.Add(MailboxAddress.Parse(user.Email));

            //email.Subject = "User Verification";
            //var builder = new BodyBuilder();
            //builder.TextBody = otpCode;
            //email.Body = builder.ToMessageBody();

            //using var smtp = new SmtpClient();
            //smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
            
            //smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            //await smtp.SendAsync(email);
            //smtp.Disconnect(true);

            //await context.SaveChangesAsync();

            //return ReturnedResponse.SuccessResponse("Email sent", null);
        }   
    }
}
