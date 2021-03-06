﻿using System;

namespace FrausDNS
{
    /// <summary>
    /// A request that was captured.
    /// </summary>
    public class CapturedRequest
    {
        /// <summary>
        /// Creates an instance of a captured request object.
        /// </summary>
        /// <param name="_data">The data that was sent to the local DNS server.</param>
        public CapturedRequest(byte[] _data)
        {
            data = _data;
        }

        public DateTime Time { get; set; }
        public string DomainRequested { get; set; }
        public string DnsReturned { get; set; }

        public byte[] data;
    }
}
