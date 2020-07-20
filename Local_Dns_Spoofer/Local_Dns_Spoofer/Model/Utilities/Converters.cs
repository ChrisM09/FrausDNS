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
        /// Future: Modify it to give a string back to put for hex view.
        /// </summary>
        /// <param name="input">The unformatted hex string.</param>
        /// <remarks>
        /// Modified https://github.com/AdvancedHacker101/c-sharp-Dns-Server 's code
        /// to work with this application.
        /// </remarks>
        public static void formatHex(string input)
        {
            int counter = 0;
            int is2 = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (counter != 0 && counter % 16 == 0)
                    Console.Write("  ");

                if (counter > 31)
                {
                    counter = 0;
                    Console.Write("\n");
                }

                Console.Write(input[i]);

                if (is2 == 1)
                {
                    is2 = -1;
                    Console.Write(" ");
                }
                counter++;
                is2++;
            }
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
            int counter = 0;
            string fullDump = "";
            foreach (int byt in input)
            {
                if (counter > 15)
                {
                    counter = 0;
                }

                string hex = byt.ToString("X");
                if (hex.Length == 1) hex = "0" + hex;
                fullDump += hex;
                counter++;
            }

            return fullDump;
        }




    }
}
