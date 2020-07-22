using DNS.Client.RequestResolver;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer
{
    public class DnsSpoofer
    {

        public DnsSpoofer(string IP = "127.0.0.1")
        {
            TargetIP = IP;
        }

        /// <summary>
        /// IP to spoof DNS requests with.
        /// </summary>
        public string TargetIP { get; set; }


        /// <summary>
        /// Server to handle all the requests through various specifications.
        /// </summary>
        private DnsServer _server;


        /// <summary>
        /// Begins the server and will report back with any requests it finds.
        /// </summary>
        /// <param name="CaptureRequest_progress">The caller to send a captured request back to.</param>
        /// <returns>A Task indicating if the server execution is completed.</returns>
        public async Task Start(IProgress<CapturedRequest> CaptureRequest_progress, IProgress<string> Error_progress)
        {
            LocalRequestResolver localRequestResolver = new LocalRequestResolver() { _TargetIP = TargetIP};
            _server = new DnsServer(localRequestResolver);

            // Whenenver the server responds, add an entry into the output list
            _server.Responded += (sender, e) =>
            {
                // Format the request here.
                // Currently prints out a lot but look at the question record and get the name.
                CapturedRequest request = new CapturedRequest()
                    { DnsReturned = "3", DomainRequested = e.Request.Questions[0].Name.ToString(), Time = DateTime.Now };

                // Send the newly captured request back for logging
                CaptureRequest_progress.Report(request);


                // Testing purposes
                Error_progress.Report( "The reponse code given back was " +e.Response.ResponseCode.ToString());


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


        /// <summary>
        /// The resolver that will spoof the requests to a specified IP address.
        /// </summary>
        public class LocalRequestResolver : IRequestResolver
        {

            // NXDOMAIN count would have to be here. If the 


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
                        //response.ResponseCode = ResponseCode.NameError; // NXDOMAIN. Now we can try to get back the IPENDPOINT to see if the request is by the person and go from there.
                        response.AnswerRecords.Add(record);
                    }
                }

                return Task.FromResult(response);
            }

        }
    }

}
