using FYPBackEnd.Data.Enums;
using FYPBackEnd.Data.ReturnedResponse;
using System.Text.RegularExpressions;
using System;

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
                return ReturnedResponse.ErrorResponse("Password must contain special character", null);
            }

            if (s.Length < 8)
            {
                return ReturnedResponse.ErrorResponse("Password must be greater than 8 characters", null);
            }

            if (!upperCase.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain at least upper case character", null);
            }

            if (!lowerCase.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain at least lower case character", null);
            }

            if (!number.IsMatch(s))
            {
                return ReturnedResponse.ErrorResponse("Password must contain at least one number", null);
            }

            return ReturnedResponse.SuccessResponse(null, true);
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

