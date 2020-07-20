using DNS.Client.RequestResolver;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer.Model
{
    public class RequestSpoofer : IRequestResolver
    {
        /// <summary>
        /// IP to spoof DNS requests with.
        /// </summary>
        public string TargetIP = "127.0.0.1";






        /// <summary>
        /// The task that will spoof all results.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A task that has a response.</returns>
        public Task<IResponse> Resolve(IRequest request, CancellationToken token = default)
        {
            IResponse response = Response.FromRequest(request);

            foreach(Question question in response.Questions)
            {
                if(question.Type == RecordType.A)
                {
                    IResourceRecord record = new IPAddressResourceRecord(question.Name, IPAddress.Parse(TargetIP));
                    response.AnswerRecords.Add(record);
                }
            }

            return Task.FromResult(response);
        }


    }
}
