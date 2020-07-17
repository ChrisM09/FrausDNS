using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Shared Functionality
        public MainWindowViewModel()
        {
            Requests = new List<CapturedRequest>();

            Requests.Add(new CapturedRequest() { Time = new DateTime(1, 1, 1), DomainRequested = "google.com", DnsReturned = "FOUND" });

            OutputString = "[+] This is a test run.\nThis is with the new line attached.";

            UpdateOutput("[+] FUNCTION TEST");

            GetNetworkAdapters();

            HeaderString = "[Request] Request Packet";
            UpdateHexViewOutput("HEX UPDATE TEST");

        }

        #endregion



        #region Capture Window Functionality

        public List<CapturedRequest> Requests { get; set; }

        public List<string> NetworkInterfaces { get; set; }

        public string SelectedInterface { get; set; }



        public string OutputString { get; set; }

        private void UpdateOutput(string text)
        {
            OutputString += "\n" + text + "\n" + "\n" + "\n" + "adsfadsfadsfgasdfgadsfgsadfgsadfgsadfgsadfgafgasdfga";
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
