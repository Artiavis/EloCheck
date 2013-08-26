using System;
using System.Text;
using System.Text.RegularExpressions;

namespace EloCheck
{
    class EloCheckUtility
    {
        // TODO what is the point of this function???
        public static string ToUpperCase(string input)
        {
            String output = "";
            input = Regex.Replace(input.ToLowerInvariant(),"[^A-Za-z]"," ");
            String[] words = input.Split(' ');
            foreach( String word in words) 
            {
                output += Char.ToUpperInvariant(word[0]) + word.Substring(1) + " ";
            }
            return output.Substring(0, output.Length - 1);
        }

        /// <summary>
        /// A helper function for parsing a roman numeral to an integer
        /// using the Numeral enum.
        /// </summary>
        /// <param name="romanNumeral">A string containing a roman numeral from 1 to 5</param>
        /// <returns>The integer value of the numeral.</returns>
        public static int RomanNumeralToInt(string romanNumeral)
        {
            romanNumeral.Trim();
            Numeral result;

            // Attempt to convert romanNumeral to its integer by name
            if (Enum.TryParse(romanNumeral, true, out result))
                if (Enum.IsDefined(typeof(Numeral), result))
                    return (int) result;
                else
                    return -1;
            else
                return -1;
        }

        /// <summary>
        /// A helper Enum for translating Roman Numerals to integers.
        /// In theory this can be replaced with a config file.
        /// </summary>
        private enum Numeral { I = 1, II = 2, III = 3, IV = 4, V = 5 };
    }
}
