using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer
{
    public class Utilities
    {

        public static List<string> GetNetworkInterfaces()
        {
            List<string> entries = new List<string>();
            foreach(NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                entries.Add(nic.Name);
            }

            return entries;
        }

    }
}
