using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace FrausDNS
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
        public static bool ChangeLocalDnsServer(string interface_name, out string errorMessage, out List<string> originalDNSAddresses, out bool autoDNS)
        {
            bool orderChanged = false;

            // Get the actual network interface to work with.
            List<NetworkInterface> nics = NetworkInterface.GetAllNetworkInterfaces()
                .Where((i) => (i.Name == interface_name)
                ).ToList();

            // Error checking
            if(nics == null)
            {
                errorMessage = "[!] There is not an active network interface with the name " + interface_name;
                originalDNSAddresses = null;
                autoDNS = false;
                return orderChanged;
            }

            NetworkInterface currentInterface = nics[0];
            IPInterfaceProperties ipProperties = currentInterface.GetIPProperties();
            List<IPAddress> originalIPs = ipProperties.DnsAddresses.ToList();

            List<string> tempOriginalIPs = new List<string>();
            foreach(IPAddress ip in originalIPs)
            {
                tempOriginalIPs.Add(ip.ToString());
            }
            originalDNSAddresses = tempOriginalIPs;

            autoDNS = DNSAuto(currentInterface.Id); 

            // We found the interface here so now we can continue with changing.
            ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection MOC = MC.GetInstances();
            string localDNSUpdatedAddress = "";

            foreach(ManagementObject MO in MOC)
            {
                if((MO["Description"].ToString().Equals(currentInterface.Description)))
                {
                    // Possible minute use case is that if two things have the same description
                    // Could overwrite multiple and one may be messed. 
                    // For now, just keep it like this.
                    if((bool)MO["IPEnabled"])
                    {
                        ManagementBaseObject dns = MO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (dns != null)
                        {
                            if (currentInterface.Supports(NetworkInterfaceComponent.IPv4))
                            {
                                // IPv4 loopback address
                                localDNSUpdatedAddress = "127.0.0.1";
                                string[] Dns = { "127.0.0.1" };
                                dns["DNSServerSearchOrder"] = Dns;
                                MO.InvokeMethod("SetDNSServerSearchOrder", dns, null);
                                orderChanged = true;
                            }
                            else
                            {
                                errorMessage = $"[-] The interface {currentInterface.Description} does not support IPv4 addresses.\n[-] This needs to be enabled. Please enable or select another interface.\n[-] Dns redirection NOT SUCCESSFUL!\n[!] Recommendation: Do NOT run the malware. The environment is unsafe.";
                                return orderChanged;
                            }
                        }
                    }
                    else
                    {
                        // This means that there is no IP connectivity here so the redirection cannot be done.
                        errorMessage = $"[-] This interface does not have internet connectivity. Please choose another option.\n[-] Dns redirection NOT SUCCESSFUL!\n[!] Recommendation: Do NOT run the malware. The environment is unsafe.";
                        return orderChanged;
                    }
                }

            }

            if (orderChanged)
            {
                // No errors have occured here. DNS set successfully.
                errorMessage = $"[+] Dns redirection complete\n[+] Local DNS Server is now localhost {localDNSUpdatedAddress} on {interface_name}";
            }
            else
            {
                // An uncommon error occurred. Prompt user with warning message.
                errorMessage = $"[-] Dns redirection INCOMPLETE\n[!] Recommendation: Do NOT run the malware. The environment is unsafe.";
            }

            return orderChanged;
        }

        /// <summary>
        /// Resets the local DNS server search order back to nothing. 
        /// </summary>
        /// <param name="interface_name">Interface to change.</param>
        /// <param name="errorMessage">Status message that will contain either an error or a pass message.</param>
        public static void ResetLocalDnsServer(string interface_name, List<string> original_settings, out string errorMessage, bool autoDNS)
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
                            if (autoDNS)
                                dns["DNSServerSearchOrder"] = null;
                            else
                                dns["DNSServerSearchOrder"] = original_settings.ToArray();

                            MO.InvokeMethod("SetDNSServerSearchOrder", dns, null);
                        }
                    }
                }
            }

            // Reset successful.
            errorMessage = "[+] Dns reset complete. Dns search order restored to original settings.";
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

        /// <summary>
        /// Determines if the "Obtain DNS server address automatically" option is enabled in the specified network adapter.
        /// </summary>
        /// <param name="NetworkAdapterGUID">The ID of the desired network adapter.</param>
        /// <returns>True or false if the option is enabled.</returns>
        private static bool DNSAuto(string NetworkAdapterGUID)
        {
            string path = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces\\" + NetworkAdapterGUID;
            string ns = (string)Registry.GetValue(path, "Nameserver", null);
            return string.IsNullOrEmpty(ns);

        }

    }
}
