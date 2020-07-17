using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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


        public static void ChangeLocalDnsServer(string interface_name, string DnsString, out string errorMessage)
        {
            string[] Dns = { DnsString };

            // Get the actual network interface to work with.
            List<NetworkInterface> nics = NetworkInterface.GetAllNetworkInterfaces()
                .Where((i) => (i.Name == interface_name)
                ).ToList();

            // Error checking
            if(nics == null)
            {
                errorMessage = "[!] There is not an active network interface with the name " + interface_name;
            }

            NetworkInterface currentInterface = nics[0];

            // We found the interface here so now we can continue with changing.
            ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection MOC = MC.GetInstances();

            foreach(ManagementObject MO in MOC)
            {
                if((bool)MO["IPEnabled"])
                {
                    // Possible minute use case is that if two things have the same description
                    // Could overwrite multiple and one may be messed. 
                    // For now, just keep it like this.
                    if(MO["Description"].ToString().Equals(currentInterface.Description))
                    {
                        ManagementBaseObject dns = MO.GetMethodParameters("SetDNSServerSearchOrder");
                        if(dns != null)
                        {
                            dns["DNSServerSearchOrder"] = Dns;
                            MO.InvokeMethod("SetDNSServerSearchOrder", dns, null);
                        }
                    }
                }
            }

            // No errors have occured here. DNS set successfully.
            errorMessage = "[+] Dns redirection complete. Dns server now " + DnsString;
        }


        public static void ResetLocalDnsServer(string interface_name, out string errorMessage)
        {
            // Get the actual network interface to work with.
            List<NetworkInterface> nics = NetworkInterface.GetAllNetworkInterfaces().Where((i) => 
                    (i.Name == interface_name)).ToList();

            // Error checking
            if (nics == null)
            {
                errorMessage = "[!] There is not an active network interface with the name " + interface_name;
            }

            NetworkInterface currentInterface = nics[0];

            // We found the interface here so now we can continue with changing.
            ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection MOC = MC.GetInstances();

            foreach(ManagementObject MO in MOC)
            {
                if((bool)MO["IPEnabled"])
                {
                    if(MO["Description"].ToString().Equals(currentInterface.Description))
                    {
                        ManagementBaseObject dns = MO.GetMethodParameters("SetDNSServerSearchOrder");
                        if(dns != null)
                        {
                            dns["DNSServerSearchOrder"] = null;
                            MO.InvokeMethod("SetDNSServerSearchOrder", dns, null);
                        }
                    }
                }
            }

            // Reset successful. Not to original specs but reset.
            errorMessage = "[+] Dns reset complete. Dns search order now empty.";

        }

    }
}
