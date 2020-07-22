using DNS.Protocol;
using Local_Dns_Spoofer.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.RightsManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Local_Dns_Spoofer.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {

        #region Shared Functionality
        public MainWindowViewModel()
        {
            UserTargetIP = "8.8.8.8";
            UserNXDOMAIN = "0";

            StartServer = new RelayCommand(startServer);
            StopServer = new RelayCommand(stopServer);

            Requests = new ObservableCollection<CapturedRequest>();

            Requests.Add(new CapturedRequest() { Time = new DateTime(1, 1, 1), DomainRequested = "google.com", DnsReturned = "FOUND" });

            //for (int i = 0; i < 200; i++)
            //{
            //    Requests.Add(new CapturedRequest() { Time = new DateTime(1, 1, 1), DomainRequested = "google.com", DnsReturned = "FOUND" });
            //}



            GetNetworkAdapters();

            //string error_message;
            //Utilities.ChangeLocalDnsServer(NetworkInterfaces[0], out error_message);


            HeaderString = "[Request] Request Packet";
            UpdateHexViewOutput("HEX UPDATE TEST");

        }

        #endregion



        #region Capture Window Functionality

        private DnsSpoofer _spoofer { get; set; }

        private Progress<CapturedRequest> progress { get; set; }

        ///// <summary>
        ///// Private list of captured requests for use in 
        ///// </summary>
        //private ListCapturedRequests _capRequests { get; set; } = new ListCapturedRequests();



        //public ListCapturedRequests CapRequests { 
        //    get
        //    {
        //        return _capRequests;
        //    }
        //    set
        //    {
        //        Requests = _capRequests.requests;
        //    }
        //}



        public ObservableCollection<CapturedRequest> Requests { get; set; }





        public List<string> NetworkInterfaces { get; set; }

        public string SelectedInterface { get; set; }





        public string UserTargetIP { get; set; }  
        public string UserNXDOMAIN { get; set; }  


        public string OutputString { get; set; }




        public RelayCommand StartServer { get; set; }
        public RelayCommand StopServer { get; set; }

        /// <summary>
        /// Starts running the DNS server in the background. Changes the LocalDNSServer to the specified IP.
        /// </summary>
        private async void startServer()
        {

            // Print the configuration options here to the console.
            UpdateOutput($"Using IP address {UserTargetIP} for DNS replies.");

            // Change the local DNS Server to localhost on a given NW interface.
            string error_message;
            Utilities.ChangeLocalDnsServer(NetworkInterfaces[0], out error_message);
            UpdateOutput(error_message);


            // DNS Server updated to localhost.

            // Print out a line saying "Sending x NXDOMAIN replies to clients.
            UpdateOutput($"Sending {UserNXDOMAIN} NXDOMAIN replies to clients.");

            progress = new Progress<CapturedRequest>();
            progress.ProgressChanged += Progress_ProgressChanged;



            // Fire and forget type deal. Have it running in the background?
            _spoofer = new DnsSpoofer(UserTargetIP);
            
            // Start will need to have a progress associated with it to
            // update the DataGrid.

            


            await _spoofer.Start(progress);




            // Once all the way done, Then print out "Server started at DateTime.Now
            UpdateOutput("Server Exection Completed.!");
        }

        private void Progress_ProgressChanged(object sender, CapturedRequest e)
        {
            Requests.Add(e);
        }

        /// <summary>
        /// Stops running the DNS server and restores original DNS search order.
        /// </summary>
        private void stopServer()
        {
            string error_message;

            Utilities.ResetLocalDnsServer(NetworkInterfaces[0], out error_message);

            UpdateOutput(error_message);

            _spoofer.Stop();

            UpdateOutput("Server Stopped. Hopefully correctly.");
        }

        private void UpdateOutput(string text)
        {
            OutputString += "\n" + text;
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
