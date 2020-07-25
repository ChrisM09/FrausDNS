﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer
{
    /// <summary>
    /// Various regexes for use to check user input.
    /// </summary>
    public class Regexes
    {
        /// <summary>
        /// Regex that only accepts numerical integers that contain digits 0-9
        /// </summary>
        public static Regex IntegerOnly = new Regex(@"^\d+$");

        /// <summary>
        /// Regex that will check the form of an IPAddress entry.
        /// </summary>
        public static Regex IPAddressForm = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
    }
}
