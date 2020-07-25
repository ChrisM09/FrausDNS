using DNS.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Local_Dns_Spoofer
{
    public class Utilities
    {
        /// <summary>
        /// Gets a list of the names of the network interfaces.
        /// </summary>
        /// <returns>A list of strings containing the names of the network interfaces.</returns>
        public static List<string> GetNetworkInterfaces()
        {
            List<string> entries = new List<string>();
            foreach(NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                entries.Add(nic.Name);
            }

            return entries;
        }

        /// <summary>
        /// Changes the local DNS server to the loopback address.
        /// </summary>
        /// <param name="interface_name">Network interface to change.</param>
        /// <param name="errorMessage">Status message that will contain either an error or a pass message.</param>
        /// <remarks> Must run as an administrator to see the desired effect.</remarks>
        public static void ChangeLocalDnsServer(string interface_name, out string errorMessage)
        {
            string[] Dns = { "127.0.0.1" };

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
            errorMessage = $"[+] Dns redirection complete\n[+] Local DNS Server is now localhost (127.0.0.1) on {interface_name}";
        }

        /// <summary>
        /// Resets the local DNS server search order back to nothing. 
        /// </summary>
        /// <param name="interface_name">Interface to change.</param>
        /// <param name="errorMessage">Status message that will contain either an error or a pass message.</param>
        /// <remarks>Note that all previous DNS orderings will be lost.</remarks>
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

        /// <summary>
        /// Attempts to determine the current default DNS address.
        /// </summary>
        /// <returns>Either a pair that indicates the first NW adapter's first DNS address or nothing.</returns>
        public static Tuple<string, string> GetDefaultDnsAddress()
        {
            Tuple<string, string> result = new Tuple<string, string>("", "");
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                    if (dnsServers.Count > 0)
                    {
                        // The first NW adapter and the first DNS address is what we will send back.
                        result = new Tuple<string, string>(adapter.Name, dnsServers[0].ToString());
                        break;

                    }
                }
            }
            return result;
        }
    }
}
