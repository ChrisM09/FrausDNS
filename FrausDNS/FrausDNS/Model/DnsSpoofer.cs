using DNS.Client.RequestResolver;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FrausDNS
{
    /// <summary>
    /// A DNS spoofer that will give back a number of NXDOMAIN responses and
    /// will send a specified IP address to every request.
    /// </summary>
    public class DnsSpoofer
    {
        #region Private Members
        /// <summary>
        /// IP to spoof DNS requests with.
        /// </summary>
        private string TargetIP { get; set; }
        /// <summary>
        /// Number of NXDOMAIN responses per domain.
        /// </summary>
        private int NXDOMAINs { get; set; }

        /// <summary>
        /// Server to handle all the requests through various specifications.
        /// </summary>
        private DnsServer _server;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an instance of the DNS spoofer
        /// </summary>
        /// <param name="IP">Target IP to spoof DNS requests. Default is loopback address.</param>
        /// <param name="NXInput">Number of NXDOMAIN responses wanted per domain. Default is 0.</param>
        public DnsSpoofer(string IP = "127.0.0.1", int NXInput = 0)
        {
            TargetIP = IP;
            NXDOMAINs = NXInput;
        }

        /// <summary>
        /// Begins the server and will report back with any requests it finds.
        /// </summary>
        /// <param name="CaptureRequest_progress">The caller to send a captured request back to.</param>
        /// <returns>A Task indicating if the server execution is completed.</returns>
        public async Task Start(IProgress<CapturedRequest> CaptureRequest_progress, IProgress<string> Error_progress)
        {
            LocalRequestResolver localRequestResolver = new LocalRequestResolver() { _TargetIP = TargetIP, Num_NXDOMAIN = NXDOMAINs};
            _server = new DnsServer(localRequestResolver);

            // Whenenver the server responds, add an entry into the output list
            _server.Responded += (sender, e) =>
            {
                string dnsReturned = "";
                if (e.Response.ResponseCode == ResponseCode.NameError) dnsReturned = "NXDOMAIN";
                else if (e.Response.ResponseCode == ResponseCode.NoError) dnsReturned = "FOUND";
                else dnsReturned = e.Response.ResponseCode.ToString();

                // Format the request here.
                CapturedRequest request = new CapturedRequest(e.Data)
                    { 
                        DnsReturned = dnsReturned, 
                        DomainRequested = e.Request.Questions[0].Name.ToString(), 
                        Time = DateTime.Now 
                    };

                // Send the newly created captured request back for logging
                CaptureRequest_progress.Report(request);
            };

            // Log when server is started.
            _server.Listening += (sender, e) =>
            {
                Error_progress.Report($"[+] Server stated successfully at {DateTime.Now.ToString()}");
            };

            // Log when there's an error in the server.
            _server.Errored += (sender, e) =>
            {
                Error_progress.Report(e.Exception.Message);
            };

            await _server.Listen();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            _server.Dispose();
        }

        #endregion 

        /// <summary>
        /// The resolver that will spoof the requests to a specified IP address.
        /// </summary>
        public class LocalRequestResolver : IRequestResolver
        {
            public int Num_NXDOMAIN;
            public Dictionary<string, int> SessionList = new Dictionary<string, int>();
            public string _TargetIP;

            /// <summary>
            /// The task that will spoof all results.
            /// </summary>
            /// <param name="request">Request</param>
            /// <param name="token">Cancellation token.</param>
            /// <returns>A task that has a response.</returns>
            public Task<IResponse> Resolve(IRequest request, CancellationToken token = default)
            {
                IResponse response = Response.FromRequest(request);
                
                foreach (Question question in response.Questions)
                {
                    if (question.Type == RecordType.A)
                    {
                        IResourceRecord record = new IPAddressResourceRecord(question.Name, IPAddress.Parse(_TargetIP));

                        // Take into account the NXDOMAIN requests.
                        if (Num_NXDOMAIN > 0)
                        {
                            // The && will prevent it from accessing something that doesn't exist in the list.
                            if (SessionList.ContainsKey(question.Name.ToString()) && SessionList[question.Name.ToString()] < Num_NXDOMAIN)
                            {
                                // send NXDOMAIN Back
                                // Error code 3.
                                response.ResponseCode = ResponseCode.NameError;
                                // increment the count for that domain.
                                SessionList[question.Name.ToString()]++;
                            }
                            else if (!SessionList.ContainsKey(question.Name.ToString()))    // Prevents duplicate keys being added if count is over.
                            {
                                // Add to list with count 1 and send a NXDOMAIN response.
                                SessionList.Add(question.Name.ToString(), 1);
                                response.ResponseCode = ResponseCode.NameError;
                            }
                        }
                        // If the NXDomain number is 0 or sent over inputted NXDOMAIN number, then don't do anything to the response.
                        response.AnswerRecords.Add(record);
                    }
                }

                return Task.FromResult(response);
            }

        }
    }

}
