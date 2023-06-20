using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.ReturnedResponse;
using System.Text.RegularExpressions;
using System;
using System.Drawing;
using FYPBackEnd.Data.Models;
using FYPBackEnd.Data.Constants;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FYPBackEnd.Core
{
    public static class Utility
    { 
        public static string FormatPhoneNumber(string phoneNo)
        {
            if (string.IsNullOrEmpty(phoneNo))
            {
                return null;
            }

            phoneNo = phoneNo.Trim().Replace(" ", "");

            if (phoneNo.Length < 10)
            {
                return null;
            }

            var tenDigitPhoneNumber = phoneNo.Substring(phoneNo.Length - 10, 10);
            return $"234{tenDigitPhoneNumber}";
        }

        public static string GenerateAlphanumericCode(int numbersToGenerate)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[numbersToGenerate];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }

        public static string GenerateOtpCode()
        {
            var randomInt = new Random();
            int otpCode = randomInt.Next(10000, 100000);
            return otpCode.ToString();
        }

        //public static bool CheckExternalUser(string userRole)
        //{
        //    if (userRole == Status.UserRole.SuperAgent.ToString() || userRole == Status.UserRole.Agent.ToString() || userRole == Status.UserRole.SubSuperAgent.ToString()
        //        || userRole == Status.UserRole.Merchant.ToString() || userRole == Status.UserRole.Mfb.ToString() || userRole == Status.UserRole.Payout.ToString()
        //        || userRole == Status.UserRole.SubMerchant.ToString())
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public static ApiResponse ValidatePassword(string s)
        {
            var specialChar = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            var upperCase = new Regex(@"[A-Z]+");
            var lowerCase = new Regex(@"[a-z]+");
            var number = new Regex(@"[0-9]+");

            if (!specialChar.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain special character", null, Data.Constants.StatusCodes.ModelError);
            }

            if (s.Length < 8)
            {
                return ReturnedResponse.ErrorResponse("Password must be greater than 8 characters", null, Data.Constants.StatusCodes.ModelError);
            }

            if (!upperCase.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain at least upper case character", null, Data.Constants.StatusCodes.ModelError);
            }

            if (!lowerCase.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain at least lower case character", null, Data.Constants.StatusCodes.ModelError);
            }

            if (!number.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain at least one number", null, Data.Constants.StatusCodes.ModelError);
            }

            return ReturnedResponse.SuccessResponse(null, true, Data.Constants.StatusCodes.Successful);
        }

        //private IFormFile ResizeImage(Image image, int maxWidth, int maxHeight)
        //{
        //    int width = image.Width;
        //    int height = image.Height;

        //    // Calculate the aspect ratio
        //    double aspectRatio = (double)width / height;

        //    // Calculate the new dimensions while maintaining the aspect ratio
        //    if (width > maxWidth || height > maxHeight)
        //    {
        //        if (aspectRatio > 1)
        //        {
        //            width = maxWidth;
        //            height = (int)(width / aspectRatio);
        //        }
        //        else
        //        {
        //            height = maxHeight;
        //            width = (int)(height * aspectRatio);
        //        }
        //    }

        //    // Create a new bitmap with the desired dimensions
        //    Bitmap resizedImage = new Bitmap(width, height);

        //    // Resize the image using Graphics
        //    using (Graphics graphics = Graphics.FromImage(resizedImage))
        //    {
        //        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        graphics.DrawImage(image, 0, 0, width, height);
        //    }

        //    return resizedImage;
        //}


        public static Image ResizeImage(IFormFile file, int maxWidth, int maxHeight)
        {
            using (var image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(ctx => ctx.Resize(maxWidth, maxHeight));
                var clonedImage = image.Clone(context =>
                {
                    context.Resize(maxWidth, maxHeight);
                });
                return clonedImage;
            }
        }
        public static bool ValidatePin(string s)
        {
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i - 1] != s[i])
                {
                    return true;
                }

                if (i == s.Length - 1 && s[i - 1] == s[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}

