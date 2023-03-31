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
                return ReturnedResponse.SuccessResponse("Email sent", null,null);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                return ReturnedResponse.ErrorResponse("Couldn't send email", null,null);
            }
            
        }

        public async Task<ApiResponse> SendVerificationEmailAsync(ApplicationUser user,string otpPurpose)
        {

            string firstName = user.FirstName;
            var otpCode = await otpService.GenerateOtpCodeAsync();

            string body = $"<table role=\"presentation\" border=\"0\" cellpadding=\"1\" cellspacing=\"0\" width=\"100%\">\r\n    <tbody><tr>\r\n      <td style=\"padding:10px 0\">\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"500\" style=\"border-collapse:collapse;border:1px solid #f6f6f6;background-color:#fff\">\r\n          <tbody><tr>\r\n            <td align=\"center\" bgcolor=\"#2D0051\" style=\"padding:52px 125px\">\r\n              <img src=\"https://ci4.googleusercontent.com/proxy/pbzO1QK2ncyAgy_tpnQD4Me_MOyrSQt_Y7GNEG0FyAiNZTyzJRoKVH7IT3FPruQL86ifRTH-Peu5s6c0PJOb8mKeKz4vIQ=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/Group.png\" alt=\"BebsPay\" width=\"70\" height=\"70\" style=\"display:block\" class=\"CToWUd\" data-bit=\"iit\">\r\n            </td>\r\n          </tr>\r\n          <tr>\r\n            <td align=\"center\" style=\"padding:35px 45px 5px 45px\">\r\n              <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse\">\r\n                <tbody><tr>\r\n                  <td align=\"center\" style=\"padding-top:30px\">\r\n                    <h1 style=\"margin:0;font-size:24px;color:#000000;font-family:Cir-bold\">Hi, {firstName} </h1>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td align=\"center\" style=\"padding:20px 50px;font-family:Cir-reg;color:#a9a5af\">\r\n                    <h6 style=\"margin:0;font-size:14px\">Please use this OTP to validate your Phonenumber on\r\n                      BebsPay.</h6>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td align=\"center\" style=\"padding:20px 50px;color:#a9a5af\">\r\n                    <p style=\"margin:0;font-size:14px;font-family:Cir-bold\">OTP details below.</p>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td align=\"center\" style=\"padding:20px;color:#004e92\">\r\n                    <p style=\"margin:0;font-size:28px;letter-spacing:10px;font-family:Cir-bold\">{otpCode}</p>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td style=\"padding:80px 90px 30px 90px\">\r\n                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse\">\r\n                      <tbody><tr>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://www.instagram.com/bebspay/\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=http://url318.errandpay.com/ls/click?upn%3Dcg6kz-2F1Tb8aWDRd7Txz-2BbihI1-2Bh5SnWdx6mNMpOOZEuFza4ymGPr0oBl8J35s-2BMxdFXB_K2whpHlVd19JRSgpX2OLnFRn3O1azTPi5wSBibU0wF7bI-2BSCukhCgC7wZXXKClOQJ-2Bim18mfzhwNm5mAM4-2F1otrvsLKUT2BFDkLGDWE-2F-2BZzfOG0jKh7mDvCKiac7h9ib6JQ3UaMKryK6VAKfMjotxFbbMtDU3saSKeoU61GOPQXNFjxqIoNxH8Odexd2ru4JKOWMjZPB27MNhJnbzT0hVJhILo3Y4FMyi2cDXJevzHc-3D&amp;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw0xKIDCZE6wnW9hrtBgklmU\">\r\n                            <img src=\"https://ci5.googleusercontent.com/proxy/0Y-31oRKoJQbb3j2TZQfKiX-cBHfrw621XyjpCoo5W6NuxzeHZNUbLGpZonLNFVob4ZVY3RwOGLUqz20-vnXc62mHRI0HIECLJo=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/instagram.png\" alt=\"instagram\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                        <td style=\"font-size:0;line-height:0\" width=\"20\">&nbsp;</td>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://web.facebook.com/profile.php?id=100090233760892\" target=\"_blank\" data-saferedirecturl=\"https://web.facebook.com/profile.php?id=100090233760892;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw2XBEiZvNYvTZNbtz57ihF5\">\r\n                            <img src=\"https://ci6.googleusercontent.com/proxy/nphgWKyEY9CHTVM_92Eop7w4pkLqFxnjplPmPT1PSNUYJYBUxRIKzMeq2LEwPqfds3UcXtZvdw2Ibh0JWx9CVN2hS-WHX71fPA=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/facebook.png\" alt=\"facebook\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                        <td style=\"font-size:0;line-height:0\" width=\"20\">&nbsp;</td>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://www.linkedin.com/company/bebspay/\" target=\"_blank\" data-saferedirecturl=\"https://www.linkedin.com/company/bebspay/;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw3cqp7Vm6d0u1-ouoaOMTRE\">\r\n                            <img src=\"https://ci4.googleusercontent.com/proxy/DcQeEdmpthZDG5XlFgfLzFFrBWi3UzAeafCnEhiYfA_ct9U_ICXJQWmwD_fzzBm_NwnPFpdgg0g5RjLemAXTl1mUY0RgHJu0fA=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/linkedin.png\" alt=\"linkedin\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                        <td style=\"font-size:0;line-height:0\" width=\"20\">&nbsp;</td>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://twitter.com/BebsPay\" target=\"_blank\" data-saferedirecturl=\"https://twitter.com/BebsPay;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw0ubGnTQB9UvEf4cnKL-5gK\">\r\n                            <img src=\"https://ci3.googleusercontent.com/proxy/b0uL7scO10M66KvwZl9yGq6sdFc_l5Ka72lk_9OecZNdQTogBuUWJVKatgf-x33ANOkRrEZGTU2byr7DZH2b0kBiHm5CkihW=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/twitter.png\" alt=\"twitter\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                      </tr>\r\n                    </tbody></table>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td style=\"padding:30px 35px 0 35px\">\r\n                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse\">\r\n                      <tbody><tr>\r\n                        <td style=\"padding-bottom:10px\">\r\n                          <p style=\"margin:0;line-height:20px;text-align:center;color:#adb6ca;font-size:15px;font-family:Cir-reg\">\r\n                            BebsPay</p>\r\n                        </td>\r\n                      </tr>\r\n                      <tr>\r\n                        <td style=\"padding-bottom:10px\">\r\n                          <p style=\"margin:0;line-height:20px;text-align:center;color:#adb6ca;font-size:15px;font-family:Cir-reg\">\r\n                            Lagos,Nigeria <br>\r\n                      </p>\r\n                        </td>\r\n                      </tr>\r\n                    </tbody></table>\r\n                  </td>\r\n                </tr>\r\n              </tbody></table>\r\n            </td>\r\n          </tr>\r\n        </tbody></table>\r\n      </td>\r\n    </tr>\r\n\r\n  </tbody></table>";

            
            var otpByUser = await context.Otps.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (otpByUser != null)
            {

                otpByUser.OtpCode = otpCode;
                otpByUser.Purpose = otpPurpose;
                otpByUser.ExpiryDate = DateTime.Now.AddMinutes(10);
                otpByUser.IsUsed = false;
                otpByUser.LastModifiedDate = DateTime.Now;

                context.Entry(otpByUser).State = EntityState.Modified;
                await context.SaveChangesAsync();

            }
            else
            {
                var otp = new Otp()
                {
                    id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    OtpCode = otpCode,
                    ExpiryDate = DateTime.Now.AddMinutes(10),
                    IsUsed = false,
                    Purpose = otpPurpose,
                    LastModifiedDate = DateTime.Now,
                    Email = user.Email,
                    UserId = user.Id
                };

                await context.Otps.AddAsync(otp);
            }

            

            MailAddress to = new MailAddress(user.Email);
            MailAddress from = new MailAddress(_mailSettings.Mail,_mailSettings.DisplayName);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "User Verification";
            message.Body =body;
            message.IsBodyHtml= true;
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
                return ReturnedResponse.SuccessResponse("Email sent", null,null);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                return ReturnedResponse.ErrorResponse("Couldn't send email", null,null);
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

        public async Task<ApiResponse> SendForgotPasswordEmailAsync(ApplicationUser user, string otpPurpose)
        {

            string firstName = user.FirstName;
            var otpCode = await otpService.GenerateOtpCodeAsync();

            string body = $"<table role=\"presentation\" border=\"0\" cellpadding=\"1\" cellspacing=\"0\" width=\"100%\">\r\n    <tbody><tr>\r\n      <td style=\"padding:10px 0\">\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"500\" style=\"border-collapse:collapse;border:1px solid #f6f6f6;background-color:#fff\">\r\n          <tbody><tr>\r\n            <td align=\"center\" bgcolor=\"#2D0051\" style=\"padding:52px 125px\">\r\n              <img src=\"https://ci4.googleusercontent.com/proxy/pbzO1QK2ncyAgy_tpnQD4Me_MOyrSQt_Y7GNEG0FyAiNZTyzJRoKVH7IT3FPruQL86ifRTH-Peu5s6c0PJOb8mKeKz4vIQ=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/Group.png\" alt=\"BebsPay\" width=\"70\" height=\"70\" style=\"display:block\" class=\"CToWUd\" data-bit=\"iit\">\r\n            </td>\r\n          </tr>\r\n          <tr>\r\n            <td align=\"center\" style=\"padding:35px 45px 5px 45px\">\r\n              <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse\">\r\n                <tbody><tr>\r\n                  <td align=\"center\" style=\"padding-top:30px\">\r\n                    <h1 style=\"margin:0;font-size:24px;color:#000000;font-family:Cir-bold\">Hi, {firstName} </h1>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td align=\"center\" style=\"padding:20px 50px;font-family:Cir-reg;color:#a9a5af\">\r\n                    <h6 style=\"margin:0;font-size:14px\">Please use this OTP to reset your password on\r\n                      BebsPay.</h6>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td align=\"center\" style=\"padding:20px 50px;color:#a9a5af\">\r\n                    <p style=\"margin:0;font-size:14px;font-family:Cir-bold\">OTP details below.</p>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td align=\"center\" style=\"padding:20px;color:#004e92\">\r\n                    <p style=\"margin:0;font-size:28px;letter-spacing:10px;font-family:Cir-bold\">{otpCode}</p>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td style=\"padding:80px 90px 30px 90px\">\r\n                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse\">\r\n                      <tbody><tr>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://www.instagram.com/bebspay/\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=http://url318.errandpay.com/ls/click?upn%3Dcg6kz-2F1Tb8aWDRd7Txz-2BbihI1-2Bh5SnWdx6mNMpOOZEuFza4ymGPr0oBl8J35s-2BMxdFXB_K2whpHlVd19JRSgpX2OLnFRn3O1azTPi5wSBibU0wF7bI-2BSCukhCgC7wZXXKClOQJ-2Bim18mfzhwNm5mAM4-2F1otrvsLKUT2BFDkLGDWE-2F-2BZzfOG0jKh7mDvCKiac7h9ib6JQ3UaMKryK6VAKfMjotxFbbMtDU3saSKeoU61GOPQXNFjxqIoNxH8Odexd2ru4JKOWMjZPB27MNhJnbzT0hVJhILo3Y4FMyi2cDXJevzHc-3D&amp;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw0xKIDCZE6wnW9hrtBgklmU\">\r\n                            <img src=\"https://ci5.googleusercontent.com/proxy/0Y-31oRKoJQbb3j2TZQfKiX-cBHfrw621XyjpCoo5W6NuxzeHZNUbLGpZonLNFVob4ZVY3RwOGLUqz20-vnXc62mHRI0HIECLJo=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/instagram.png\" alt=\"instagram\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                        <td style=\"font-size:0;line-height:0\" width=\"20\">&nbsp;</td>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://web.facebook.com/profile.php?id=100090233760892\" target=\"_blank\" data-saferedirecturl=\"https://web.facebook.com/profile.php?id=100090233760892;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw2XBEiZvNYvTZNbtz57ihF5\">\r\n                            <img src=\"https://ci6.googleusercontent.com/proxy/nphgWKyEY9CHTVM_92Eop7w4pkLqFxnjplPmPT1PSNUYJYBUxRIKzMeq2LEwPqfds3UcXtZvdw2Ibh0JWx9CVN2hS-WHX71fPA=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/facebook.png\" alt=\"facebook\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                        <td style=\"font-size:0;line-height:0\" width=\"20\">&nbsp;</td>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://www.linkedin.com/company/bebspay/\" target=\"_blank\" data-saferedirecturl=\"https://www.linkedin.com/company/bebspay/;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw3cqp7Vm6d0u1-ouoaOMTRE\">\r\n                            <img src=\"https://ci4.googleusercontent.com/proxy/DcQeEdmpthZDG5XlFgfLzFFrBWi3UzAeafCnEhiYfA_ct9U_ICXJQWmwD_fzzBm_NwnPFpdgg0g5RjLemAXTl1mUY0RgHJu0fA=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/linkedin.png\" alt=\"linkedin\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                        <td style=\"font-size:0;line-height:0\" width=\"20\">&nbsp;</td>\r\n                        <td align=\"center\">\r\n                          <a href=\"https://twitter.com/BebsPay\" target=\"_blank\" data-saferedirecturl=\"https://twitter.com/BebsPay;source=gmail&amp;ust=1678206277804000&amp;usg=AOvVaw0ubGnTQB9UvEf4cnKL-5gK\">\r\n                            <img src=\"https://ci3.googleusercontent.com/proxy/b0uL7scO10M66KvwZl9yGq6sdFc_l5Ka72lk_9OecZNdQTogBuUWJVKatgf-x33ANOkRrEZGTU2byr7DZH2b0kBiHm5CkihW=s0-d-e1-ft#https://stagingapi.errandpay.com/epImages/twitter.png\" alt=\"twitter\" width=\"32\" height=\"32\" style=\"display:block\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n                          </a>\r\n                        </td>\r\n                      </tr>\r\n                    </tbody></table>\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td style=\"padding:30px 35px 0 35px\">\r\n                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse\">\r\n                      <tbody><tr>\r\n                        <td style=\"padding-bottom:10px\">\r\n                          <p style=\"margin:0;line-height:20px;text-align:center;color:#adb6ca;font-size:15px;font-family:Cir-reg\">\r\n                            BebsPay</p>\r\n                        </td>\r\n                      </tr>\r\n                      <tr>\r\n                        <td style=\"padding-bottom:10px\">\r\n                          <p style=\"margin:0;line-height:20px;text-align:center;color:#adb6ca;font-size:15px;font-family:Cir-reg\">\r\n                            Lagos,Nigeria <br>\r\n                      </p>\r\n                        </td>\r\n                      </tr>\r\n                    </tbody></table>\r\n                  </td>\r\n                </tr>\r\n              </tbody></table>\r\n            </td>\r\n          </tr>\r\n        </tbody></table>\r\n      </td>\r\n    </tr>\r\n\r\n  </tbody></table>";

            
            var otpByUser = await context.Otps.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (otpByUser != null)
            {

                otpByUser.OtpCode = otpCode;
                otpByUser.Purpose = otpPurpose;
                otpByUser.ExpiryDate = DateTime.Now.AddMinutes(10);
                otpByUser.IsUsed = false;
                otpByUser.LastModifiedDate = DateTime.Now;

                context.Entry(otpByUser).State = EntityState.Modified;
                await context.SaveChangesAsync();

            }
            else
            {
               var otp = new Otp()
                {
                    id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    OtpCode = otpCode,
                    ExpiryDate = DateTime.Now.AddMinutes(10),
                    IsUsed = false,
                    Purpose = otpPurpose,
                    LastModifiedDate = DateTime.Now,
                    Email = user.Email,
                    UserId = user.Id
                };

                await context.Otps.AddAsync(otp);
            }

            

            MailAddress to = new MailAddress(user.Email);
            MailAddress from = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "User Verification";
            message.Body = body;
            message.IsBodyHtml = true;
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
                return ReturnedResponse.SuccessResponse("Email sent", null,null);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                return ReturnedResponse.ErrorResponse("Couldn't send email", null,null);
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
