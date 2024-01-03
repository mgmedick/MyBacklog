using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Common.Extensions
{
    public static class StringExtensions
    {
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
                
        public static string GeneratePassword(int Length, int NonAlphaNumericChars)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string allowedNonAlphaNum = "._()-/#&*!$@+%^=[{]};:>|?";
            Random rd = new Random();

            if (NonAlphaNumericChars > Length || Length <= 0 || NonAlphaNumericChars < 0)
                throw new ArgumentOutOfRangeException();

            char[] pass = new char[Length];
            int[] pos = new int[Length];
            int i = 0, j = 0, temp = 0;
            bool flag = false;

            //Random the position values of the pos array for the string Pass
            while (i < Length - 1)
            {
                j = 0;
                flag = false;
                temp = rd.Next(0, Length);
                for (j = 0; j < Length; j++)
                    if (temp == pos[j])
                    {
                        flag = true;
                        j = Length;
                    }

                if (!flag)
                {
                    pos[i] = temp;
                    i++;
                }
            }

            //Random the AlphaNumericChars
            for (i = 0; i < Length - NonAlphaNumericChars; i++)
                pass[i] = allowedChars[rd.Next(0, allowedChars.Length)];

            //Random the NonAlphaNumericChars
            for (i = Length - NonAlphaNumericChars; i < Length; i++)
                pass[i] = allowedNonAlphaNum[rd.Next(0, allowedNonAlphaNum.Length)];

            //Set the sorted array values by the pos array for the rigth posistion
            char[] sorted = new char[Length];
            for (i = 0; i < Length; i++)
                sorted[i] = pass[pos[i]];

            string Pass = new String(sorted);

            return Pass;
        }

        public static string SanatizeGameName(this string gameName)
        {
            var santizedGameName = gameName.Sanatize().ReplaceRomanWithInt().ReplaceTextWithNumber().ReplaceMultiSpaceWithSingle().Trim();
            
            santizedGameName = Regex.Replace(santizedGameName, "^COD", "Call of Duty");
            santizedGameName = Regex.Replace(santizedGameName, "^GTA", "Grand Theft Auto");
            
            var valuesToTrim = new string[] { "_", "-", ":" };
            santizedGameName = santizedGameName.Replace(valuesToTrim, string.Empty);

            var valuesToReplace = new List<Tuple<string,string>> { new Tuple<string, string>("&", "and") };
            foreach (var valueToReplace in valuesToReplace)
            {
                santizedGameName = santizedGameName.Replace(valueToReplace.Item1, valueToReplace.Item2);
            }
            
            return santizedGameName;
        }

        public static string Sanatize(this string input)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(input.Normalize(NormalizationForm.FormD).Where(i => i <= 128).ToArray());

            return stringBuilder.ToString();
        }

        public static string ReplaceMultiSpaceWithSingle(this string input)
        {
            return Regex.Replace(input, @"\s+", " ");
        }

        public static string Replace(this string input, string[] separators, string newVal, StringComparison? comparer = null)
        {
            var temp = input;
            foreach(var separator in separators)
            {
                temp = temp.Replace(separator, newVal, (comparer ?? StringComparison.CurrentCulture));
            }

            return temp;
        }        

        public static string ReplaceRomanWithInt(this string input)
        {
            var matches = Regex.Matches(input, @"(?<=\W)(M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3}))(?!\w)", RegexOptions.IgnoreCase);
            
            if (matches.Any()) 
            {
                var items = matches.Cast<Match>()
                                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                .Select(x => x.Value.Trim())
                                .Distinct()
                                .ToList();
                var lookup = items.Select(i => new Tuple<string,string>(i, i.ConvertRomanToInt().ToString())).ToList();
                    
                foreach (var item in lookup)
                {
                    input = input.Replace(item.Item1, item.Item2);
                }                
            }

            return input;
        }
            
        public static int ConvertRomanToInt(this string roman)
        {
            roman = roman.ToUpper();

            var RomanMap = new Dictionary<char, int>() {
                {'I', 1},
                {'V', 5},
                {'X', 10},
                {'L', 50},
                {'C', 100},
                {'D', 500},
                {'M', 1000}
            };

            var number = 0;
            var previousChar = roman[0];
            foreach(char currentChar in roman)
            {
                number += RomanMap[currentChar];
                if(RomanMap[previousChar] < RomanMap[currentChar])
                {
                    number -= RomanMap[previousChar] * 2;
                }
                previousChar = currentChar;
            }
                
            return number;
        }

        public static string ReplaceTextWithNumber(this string input)
        {
            var matches = Regex.Matches(input, @"((?:(zero|one|two|three|four|five|six|seven|eight|nine|ten|eleven|twelve|thirteen|fourteen|fifteen|sixteen|seventeen|eighteen|nineteen|twenty|thirty|forty|fifty|sixty|seventy|eighty|ninety|hundred|thousand|million|billion|trillion|quadrillion|quintillion)(?: +|$))+)", RegexOptions.IgnoreCase);
            
            if (matches.Any()) 
            {
                var items = matches.Cast<Match>()
                                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                .Select(x => x.Value.Trim())
                                .Distinct()
                                .ToList();
                var lookup = items.Select(i => new Tuple<string,string>(i, i.ConvertTextToNumber().ToString())).ToList();
                    
                foreach (var item in lookup)
                {
                    input = input.Replace(item.Item1, item.Item2);
                }                
            }

            return input;
        }

        private static Dictionary<string,long> numberTable =
            new Dictionary<string,long>
                {{"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
                {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
                {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
                {"fourteen",14},{"fifteen",15},{"sixteen",16},
                {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
                {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
                {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
                {"thousand",1000},{"million",1000000},{"billion",1000000000},
                {"trillion",1000000000000},{"quadrillion",1000000000000000},
                {"quintillion",1000000000000000000}};

        public static long ConvertTextToNumber(this string numberString)
        {
            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                .Select(m => m.Value.ToLowerInvariant())
                .Where(v => numberTable.ContainsKey(v))
                .Select(v => numberTable[v]);
            long acc = 0,total = 0L;
            foreach(var n in numbers)
            {
                if(n >= 1000)
                {
                    total += (acc > 0 ? acc : 1) * n;
                    acc = 0;
                }
                else if(n >= 100){
                    acc = (acc > 0 ? acc : 1) * n;
                }
                else acc += n;          
            }
            return (total + acc)  * ( numberString.StartsWith("minus",
                StringComparison.InvariantCultureIgnoreCase) ? -1 : 1);
        }                                           
    }
}
