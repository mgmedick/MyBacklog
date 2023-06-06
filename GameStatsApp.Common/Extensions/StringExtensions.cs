using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Common.Extensions
{
    public static class StringExtensions
    {
        public static string[] SplitString(this string param, string delimiter, int? count = null)
        {
            string[] result = null;

            if (count.HasValue)
            {
                result = param.Split(new string[] { delimiter }, count.Value, StringSplitOptions.None);
            }
            else
            {
                result = param.Split(new string[] { delimiter }, StringSplitOptions.None);
            }
            return param.Split(new string[] { delimiter }, StringSplitOptions.None);
        }

        public static string HashString(this string pass)
        {
            string result = BCrypt.Net.BCrypt.HashPassword(pass);

            return result;
        }

        public static bool VerifyHash(this string pass, string passHash)
        {
            bool result = BCrypt.Net.BCrypt.Verify(pass, passHash);

            return result;
        }

        public static string GetHMACSHA256Hash(this string plaintext, string salt)
        {
            string result = string.Empty;
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join(string.Empty, baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
        
        public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            var punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
            if (length < 1 || length > 128)
            {
                throw new ArgumentException(nameof(length));
            }

            if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
            {
                throw new ArgumentException(nameof(numberOfNonAlphanumericCharacters));
            }

            using (var rng = RandomNumberGenerator.Create())
            {
                var byteBuffer = new byte[length];

                rng.GetBytes(byteBuffer);

                var count = 0;
                var characterBuffer = new char[length];

                for (var iter = 0; iter < length; iter++)
                {
                    var i = byteBuffer[iter] % 87;

                    if (i < 10)
                    {
                        characterBuffer[iter] = (char)('0' + i);
                    }
                    else if (i < 36)
                    {
                        characterBuffer[iter] = (char)('A' + i - 10);
                    }
                    else if (i < 62)
                    {
                        characterBuffer[iter] = (char)('a' + i - 36);
                    }
                    else
                    {
                        characterBuffer[iter] = punctuations[i - 62];
                        count++;
                    }
                }

                if (count >= numberOfNonAlphanumericCharacters)
                {
                    return new string(characterBuffer);
                }

                int j;
                var rand = new Random();

                for (j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
                {
                    int k;
                    do
                    {
                        k = rand.Next(0, length);
                    }
                    while (!char.IsLetterOrDigit(characterBuffer[k]));

                    characterBuffer[k] = punctuations[rand.Next(0, punctuations.Length)];
                }

                return new string(characterBuffer);
            }
        }      
    }
}
