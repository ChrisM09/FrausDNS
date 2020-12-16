using FrausDNS.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;

namespace FrausDNS.ViewModel
{
    public class MainWindowViewModel : BaseViewModel, IDataErrorInfo
    {

        #region Shared Functionality

        /// <summary>
        /// Creates an instance of the Main Window View Model.
        /// </summary>
        public MainWindowViewModel()
        {
            initialSetup();
        }

        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        /// Non-used error code. Needed for DataErrorInfo.
        /// </summary>
        string IDataErrorInfo.Error { get { return null; } }

        /// <summary>
        /// Report any error based on a property back to the UI and place it into the tool tip.
        /// </summary>
        /// <param name="columnName">The property that was changed within the ViewModel</param>
        /// <returns>Either an error message or null dependent if the input was valid.</returns>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error_message = null;
                switch (columnName)
                {
                    case "UserTargetIP":
                        {
                            // Check here for matching up against a certain form (regex?)
                            if (!isUserTargetIPValid())
                            {
                                error_message = "IP not in the form xxx.xxx.xxx.xxx";
                            }
                            break;
                        }
                    case "UserNXDOMAIN":
                        {
                            // Check if it's not empty, is integers only and is greater than 0
                            if (!isUserNXDOMAINValid())
                            {
                                error_message = "NXDOMAINs must have a value that is an integer greater than or equal to 0.";
                            }
                            break;
                        }
                    case "SelectedInterface":
                        {
                            // The choice is made from a drop down, so just check if it's empty.
                            if (!isSelectedInterfaceValid())
                            {
                                error_message = "An interface needs to be selected.";
                            }
                            break;
                        }
                    default:
                        {
                            error_message = "Unknown Error.";
                            break;
                        }
                }
                return error_message;
            }
        }
        #endregion

        #region Capture Window Functionality

        #region Private Capture Window members
        private DnsSpoofer _spoofer { get; set; }
        private Progress<CapturedRequest> _capturedRequestProgress { get; set; }
        private Progress<string> _errorMessageProgress { get; set; }
        private bool _serverStarted { get; set; }
        private List<string> _original_DNS_Search_Order;
        private bool _autoDNS;

        #endregion

        #region Public Capture Window Members 
        public bool StartServerEnabled
        {
            get
            {
                // Check the inputs and enable button if they're all acceptable.
                return
                    isUserTargetIPValid() &&
                    isUserNXDOMAINValid() &&
                    isSelectedInterfaceValid() &&
                    (_serverStarted == false);

            }
        }
        public bool StopServerEnabled
        {
            get
            {
                return _serverStarted == true;
            }
        }
        public bool DropdownEnabled { get; set; }
        public bool ReadOnly { get; set; }
        public ObservableCollection<CapturedRequest> Requests { get; set; }
        public List<string> NetworkInterfaces { get; set; }
        public string UserTargetIP { get; set; }
        public string UserNXDOMAIN { get; set; }
        public string SelectedInterface { get; set; }
        public string OutputString { get; set; }
        public bool serverStopped { get; set; }

        #endregion

        #region Public Capture Window Commands
        public RelayCommand StartServer { get; set; }
        public RelayCommand StopServer { get; set; }
        #endregion

        #region Private Capture Window Methods

        /// <summary>
        /// Starts running the DNS server in the background. Changes the LocalDNSServer to the specified IP.
        /// </summary>
        private async void startServer()
        {
            // Need to capture in a try-finally block because if a user shuts the service
            // down while the server is running, the DNS Server Search Order still needs to be reset.
            try
            {
                try
                {
                    IPAddress.Parse(UserTargetIP);
                }
                catch (Exception e)
                {
                    updateOutput($"[-] {e.Message}");
                    return;
                }

                // Print the configuration options here to the console.
                updateOutput($"[+] Using IP address {UserTargetIP} for DNS replies.");

                // Change the local DNS Server to localhost on a given NW interface.
                string error_message;
                Utilities.ChangeLocalDnsServer(SelectedInterface, out error_message, out _original_DNS_Search_Order, out _autoDNS);
                updateOutput(error_message);

                updateOutput($"[+] Sending {UserNXDOMAIN} NXDOMAIN replies to clients.");

                // Captured Request gotten from the server.
                _capturedRequestProgress = new Progress<CapturedRequest>();
                _capturedRequestProgress.ProgressChanged += (sender, e) => { Requests.Add(e); };

                // Error Message gotten from the server.
                _errorMessageProgress = new Progress<string>();
                _errorMessageProgress.ProgressChanged += (sender, e) => { updateOutput(e); };

                _spoofer = new DnsSpoofer(UserTargetIP, int.Parse(UserNXDOMAIN));
                _serverStarted = true;
                serverStopped = false;
                ReadOnly = true;
                DropdownEnabled = false;

                // Start the server and have it report back.
                await _spoofer.Start(_capturedRequestProgress, _errorMessageProgress);
            }
            finally
            {
                if(!serverStopped)
                    StopServer.Execute(null);
            }
        }

        /// <summary>
        /// Stops running the DNS server and restores original DNS search order.
        /// </summary>
        private void stopServer()
        {
            string error_message;

            Utilities.ResetLocalDnsServer(SelectedInterface, _original_DNS_Search_Order, out error_message, _autoDNS);

            updateOutput(error_message);

            _spoofer.Stop();

            _serverStarted = false;
            ReadOnly = false;
            DropdownEnabled = true;
            serverStopped = true;

            updateOutput("[+] Server Stopped.");
        }

        /// <summary>
        /// Updates the output console window.
        /// </summary>
        /// <param name="text">Text to display on the screen.</param>
        private void updateOutput(string text)
        {
            OutputString += text + "\n";
        }

        /// <summary>
        /// Gets the network adapters from the local machine.
        /// </summary>
        private void getNetworkAdapters()
        {
            NetworkInterfaces = Utilities.GetNetworkInterfaces();
        }

        /// <summary>
        /// Validates if the Target IP is of the valid form.
        /// </summary>
        /// <returns>Whether or not the target IP is in the correct format.</returns>
        private bool isUserTargetIPValid()
        {
            return 
                !string.IsNullOrWhiteSpace(UserTargetIP) &&
                Regexes.IPAddressForm.IsMatch(UserTargetIP);
        }

        /// <summary>
        /// Validates if the NXDOMAIN number is of the valid form.
        /// </summary>
        /// <returns>Whether or not the NXDOMAIN number is in the correct format.</returns>
        private bool isUserNXDOMAINValid()
        {
            return
                !string.IsNullOrEmpty(UserNXDOMAIN) &&
                Regexes.IntegerOnly.IsMatch(UserNXDOMAIN) &&
                int.Parse(UserNXDOMAIN) >= 0;

        }

        /// <summary>
        /// Validates if there is an interface selected.
        /// </summary>
        /// <returns>Whether or not an interface is selected.</returns>
        private bool isSelectedInterfaceValid()
        {
            return !string.IsNullOrEmpty(SelectedInterface);
        }

        /// <summary>
        /// Performs initial setup of the applcation. 
        /// </summary>
        /// <remarks>
        /// Will set up default DNS choices, set NXDOMAIN to 0, initializes commands,
        /// and gets the list of network adapters.
        /// </remarks>
        private void initialSetup()
        {
            Tuple<string, string> defaultDns = Utilities.GetDefaultDnsAddress();
            SelectedInterface = defaultDns.Item1;
            UserTargetIP = defaultDns.Item2;
            UserNXDOMAIN = "0";
            ReadOnly = false;
            DropdownEnabled = true;
            serverStopped = true;
            _original_DNS_Search_Order = new List<string>();

            StartServer = new RelayCommand(startServer);
            StopServer = new RelayCommand(stopServer);

            Requests = new ObservableCollection<CapturedRequest>();
            getNetworkAdapters();
        }
        #endregion

        #endregion

        #region DNS Hex View Functionality

        #region Private DNS Hex View Members
        private CapturedRequest _selectedRequest { get; set; }
        #endregion

        #region Public DNS Hex View Members
        public string HexByteOutput { get; set; }
        public string ConvertedOutput { get; set; }
        public string HeaderString { get; set; }
        public CapturedRequest SelectedRequest 
        {  
            get
            {
                return _selectedRequest;
            }
            set
            {
                HeaderString = value.DomainRequested + " Request Packet";
                UpdateHexViewOutput(value.data);
                _selectedRequest = value;
            }
        
        }
        #endregion

        #region Private DNS Hex View Methods

        /// <summary>
        /// Updates the console output window.
        /// </summary>
        /// <param name="replacement">The bytes that need to be translated.</param>
        private void UpdateHexViewOutput(byte[] replacement)
        {
            string hex = Converters.byteToHex(replacement);

            HexByteOutput = Converters.formatHex(hex);
            ConvertedOutput = Converters.GetSimplifiedHexDump(hex);
        }

        #endregion

        #endregion
    }

}
