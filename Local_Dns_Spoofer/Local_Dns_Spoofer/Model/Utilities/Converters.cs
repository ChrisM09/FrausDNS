using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer
{
    public class Converters
    {

        /// <summary>
        /// Formats a hex string and will print it out to the console.
        /// </summary>
        /// <param name="input">The unformatted hex string.</param>
        /// <remarks>
        /// Modified https://github.com/AdvancedHacker101/c-sharp-Dns-Server 's code
        /// to work with this application.
        /// </remarks>
        public static string formatHex(string input)
        {
            string output = "";
            int counter = 0;
            int is2 = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (counter != 0 && counter % 16 == 0)
                    output += "  ";

                if (counter > 31)
                {
                    counter = 0;
                    output += "\n";
                }

                output += input[i];

                if (is2 == 1)
                {
                    is2 = -1;
                    output += " ";
                }
                counter++;
                is2++;
            }

            return output;
        }

        /// <summary>
        /// Convertes a byte array into corresponding hex string.
        /// </summary>
        /// <param name="input">Byte array to convert.</param>
        /// <returns>Concatenated, unformatted hex string.</returns>
        /// <remarks>
        /// Modified https://github.com/AdvancedHacker101/c-sharp-Dns-Server 's code
        /// to work with this application.
        /// </remarks>
        public static string byteToHex(byte[] input)
        {
            string fullDump = "";
            foreach (int byt in input)
            {
                string hex = byt.ToString("X");
                if (hex.Length == 1) hex = "0" + hex;
                fullDump += hex;
            }

            return fullDump;
        }

        /// <summary>
        /// Will give back a simplified string of a hex dump with alphanumeric characters only.
        /// A-Z, a-z and 0-9. Anything else will turn into ".". 
        /// </summary>
        /// <param name="HexDump">The raw hex dump to print.</param>
        /// <returns>A simplified string containing the alphnumeric characters of the hex dump.</returns>
        public static string GetSimplifiedHexDump(string HexDump)
        {
            // Every two find decimal equivalent.
            int substr_start = 0;
            string converted = "";

            while(substr_start < HexDump.Length)
            {
                // Get two characters.
                string HexChar = HexDump.Substring(substr_start, 2);
                int hex_value = Int16.Parse(HexChar, System.Globalization.NumberStyles.AllowHexSpecifier);
                char result = '.';
                if((hex_value >= 65 && hex_value <= 90) || (hex_value >= 97 && hex_value <= 122) || (hex_value >= 48 && hex_value <= 57))
                {
                    result = (char)hex_value;
                }
                converted += result;
                substr_start += 2;
            }

            return converted;
        }
    }
}
