using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FYPBackEnd.Services.Implementation
{
    public class CryptoServices
    {
        private TripleDESCryptoServiceProvider TDESAlgo;
        private string keystring;

        private string _password;
        private int _salt;



        public CryptoServices()
        {
            //keystring = "B738C0DB478907CAE98CF476";
            keystring = "6DB18246D230DF0B36C01DB9";
            init();
        }

        public CryptoServices(string strPassword, int nSalt)
        {
            _password = strPassword;
            _salt = nSalt;
        }

        public CryptoServices(bool usePadding)
        {
            //keystring = "B738C0DB478907CAE98CF476";
            keystring = "6DB18246D230DF0B36C01DB9";
            init();
        }

        public CryptoServices(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (key.Length == 24)
                {
                    keystring = key;
                    init();
                }
                else
                {
                    throw new ArgumentException("key is invalid. Must be hexadecimal and 24 characters long");
                }
            }
            else
            {
                throw new ArgumentException("key is invalid. Must be hexadecimal and 24 characters long");
            }
        }

        private void init()
        {
            TDESAlgo = new TripleDESCryptoServiceProvider();
            TDESAlgo.Padding = PaddingMode.PKCS7;
            TDESAlgo.Mode = CipherMode.CBC;
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyB = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(keystring));
            hashmd5.Clear();
            TDESAlgo.Key = keyB;
        }

        public string EncryptData(string plaintext)
        {
            string output = null;
            //MemoryStream ms = new MemoryStream();
            //CryptoStream cs = new CryptoStream(ms, TDESAlgo.CreateEncryptor(), CryptoStreamMode.Write);
            //StreamWriter writer = new StreamWriter(cs);
            //writer.Write(plaintext);
            //writer.Flush();
            //writer.Close();
            //cs.Close();
            //byte[] encMsg = ms.ToArray();
            //ms.Close();

            //output = Convert.ToBase64String(encMsg);

            ICryptoTransform encrytor = TDESAlgo.CreateEncryptor();
            byte[] input = Encoding.UTF8.GetBytes(plaintext);
            byte[] encmsg = encrytor.TransformFinalBlock(input, 0, input.Length);
            output = Convert.ToBase64String(encmsg);
            return output;
        }

        public string DecryptData(string ciphertext)
        {
            string output = null;
            byte[] enc = Convert.FromBase64String(ciphertext);
            //MemoryStream ms = new MemoryStream();
            //CryptoStream cs = new CryptoStream(ms, TDESAlgo.CreateDecryptor(), CryptoStreamMode.Write);
            //StreamWriter writer = new StreamWriter(cs);
            //writer.Write(enc);
            //writer.Flush();
            //writer.Close();
            //cs.Close();
            //byte[] plainBytes = ms.ToArray();
            //ms.Close();
            //output = Encoding.UTF8.GetString(plainBytes);
            ICryptoTransform decryptor = TDESAlgo.CreateDecryptor();
            byte[] input = Convert.FromBase64String(ciphertext);
            byte[] plainBytes = decryptor.TransformFinalBlock(input, 0, input.Length);
            output = Encoding.UTF8.GetString(plainBytes);
            return output;
        }
        public static byte[] StringToByteArray(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public string DecryptTripleDES(string cipherText)
        {
            byte[] result;
            byte[] dataToDecrypt = Convert.FromBase64String(cipherText);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyB = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("6DB18246D230DF0B36C01DB9"));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider { Key = keyB, Mode = CipherMode.CBC, IV = new byte[8], Padding = PaddingMode.PKCS7 };

            using (ICryptoTransform cTransform = tdes.CreateDecryptor())
            {
                result = cTransform.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                tdes.Clear();
            }

            return Encoding.UTF8.GetString(result);
        }

        public string EncryptTripleDES(string cipherText)
        {
            byte[] byt = Encoding.UTF8.GetBytes(cipherText);
            string mdo = Convert.ToBase64String(byt);
            byte[] result;
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(cipherText);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyB = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("6DB18246D230DF0B36C01DB9"));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider { Key = keyB, Mode = CipherMode.CBC, IV = new byte[8], Padding = PaddingMode.PKCS7 };

            using (ICryptoTransform cTransform = tdes.CreateEncryptor())
            {
                result = cTransform.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                tdes.Clear();
            }

            return Convert.ToBase64String(result, 0, result.Length);
        }

        public string Encode(string clearText)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(clearText);
            return Convert.ToBase64String(encoded);
        }

        public string Decode(string encodedText)
        {
            byte[] encoded = Convert.FromBase64String(encodedText);
            return Encoding.UTF8.GetString(encoded);
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
            byte[] randomBytes = new byte[PasswordLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }

        public static int CreateRandomSalt()
        {
            byte[] _saltBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(_saltBytes);

            return (_saltBytes[0] << 24) + (_saltBytes[1] << 16) +
              (_saltBytes[2] << 8) + _saltBytes[3];
        }

        public string ComputeSaltedHash()
        {
            // Create Byte array of password string
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] _secretBytes = encoder.GetBytes(_password);

            // Create a new salt
            byte[] _saltBytes = new byte[4];
            _saltBytes[0] = (byte)(_salt >> 24);
            _saltBytes[1] = (byte)(_salt >> 16);
            _saltBytes[2] = (byte)(_salt >> 8);
            _saltBytes[3] = (byte)_salt;

            // append the two arrays
            byte[] toHash = new byte[_secretBytes.Length + _saltBytes.Length];
            Array.Copy(_secretBytes, 0, toHash, 0, _secretBytes.Length);
            Array.Copy(_saltBytes, 0, toHash, _secretBytes.Length, _saltBytes.Length);

            SHA1 sha1 = SHA1.Create();
            byte[] computedHash = sha1.ComputeHash(toHash);

            return encoder.GetString(computedHash);
        }
    }

    public class RandomCodeGeneration
    {
        // Define default min and max password lengths.
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH, false);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">
        /// Exact password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        public static string Generate(int length, bool isMerchantCode)
        {
            return Generate(length, length, isMerchantCode);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public static string Generate(int minLength,
                                      int maxLength, bool isMerchantCode, bool isAccountNumber = false)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][]
                {
                    PASSWORD_CHARS_LCASE.ToCharArray(),
                    PASSWORD_CHARS_UCASE.ToCharArray(),
                    PASSWORD_CHARS_NUMERIC.ToCharArray(),
                    PASSWORD_CHARS_SPECIAL.ToCharArray()
                };

            //Merchant code has only uppercase letters and numbers
            if (isMerchantCode)
            {
                charGroups = new char[][]
                {
                    PASSWORD_CHARS_UCASE.ToCharArray(),
                    PASSWORD_CHARS_NUMERIC.ToCharArray(),
                };
            }

            //Account Number has only numbers
            if (isAccountNumber)
            {
                charGroups = new char[][]
                {
                    PASSWORD_CHARS_NUMERIC.ToCharArray(),
                };
            }

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }


    }
}