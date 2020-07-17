using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer
{
    /// <summary>
    /// Instance of a captured request for use in the CaptureWindow
    /// </summary>
    public class CapturedRequest
    {
        public DateTime Time { get; set; }
        public string DomainRequested { get; set; }
        public string DnsReturned { get; set; }
    }
}
