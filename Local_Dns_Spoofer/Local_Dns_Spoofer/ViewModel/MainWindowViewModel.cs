using Local_Dns_Spoofer.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Security.RightsManagement;

namespace Local_Dns_Spoofer.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _targetIP;



        #region Shared Functionality
        public MainWindowViewModel()
        {
            UserTargetIP = "8.8.8.8";

            Requests = new List<CapturedRequest>();

            Requests.Add(new CapturedRequest() { Time = new DateTime(1, 1, 1), DomainRequested = "google.com", DnsReturned = "FOUND" });

            for (int i = 0; i < 200; i++)
            {
                Requests.Add(new CapturedRequest() { Time = new DateTime(1, 1, 1), DomainRequested = "google.com", DnsReturned = "FOUND" });
            }



            OutputString = "[+] This is a test run.\nThis is with the new line attached.";

            UpdateOutput("[+] FUNCTION TEST");

            GetNetworkAdapters();

            HeaderString = "[Request] Request Packet";
            UpdateHexViewOutput("HEX UPDATE TEST");

            string error_message;
            Utilities.ChangeLocalDnsServer(NetworkInterfaces[0], "127.0.0.1", out error_message);

            UpdateOutput(error_message);

            Utilities.ResetLocalDnsServer(NetworkInterfaces[0], out error_message);

            UpdateOutput(error_message);

        }

        #endregion



        #region Capture Window Functionality

        public List<CapturedRequest> Requests { get; set; }

        public List<string> NetworkInterfaces { get; set; }

        public string SelectedInterface { get; set; }



        // This will be removed soon. Just for testing purposes.
        public string UserTargetIP
        {
            get
            {
                return _targetIP;
            }
            set
            {
                OutputString += $"\n[+] IP changed from {_targetIP} to {value}";
                _targetIP = value;
            }
        }


        // public string UserTargetIP { get; set; }

        public string UserNXDOMAIN { get; set; }


        public string OutputString { get; set; }




        

        private void UpdatedIPOutput()
        {
            OutputString += $"\n[+] IP Changed to {UserTargetIP}";
        }


        private void UpdateOutput(string text)
        {
            OutputString += "\n" + text + "\n" + "\n" + "\n";
        }

        private void GetNetworkAdapters()
        {
            NetworkInterfaces = Utilities.GetNetworkInterfaces();
        }
        #endregion



        #region DNS Hex View Functionality

        public string HexByteOutput { get; set; }

        public string ConvertedOutput { get; set; }

        public string HeaderString { get; set; }



        private void UpdateHexViewOutput(string replacement)
        {
            HexByteOutput = "Hex:\t" + replacement;
            // would call a conversion method here but for now , it's a stub
            ConvertedOutput = "Regular: \t" + replacement;
        }

        #endregion




    }
}
