using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local_Dns_Spoofer.Model
{
    public class DnsRequest
    {




    }


    public class DnsHeader
    {
        // Reference used is here under section 4.1.1 "Header section format"
        // https://tools.ietf.org/html/rfc1035

        // Name of the field.                                                       // Number of bits.
        
        public short ID;                                                             // [16]

        #region Flags

        public int QR;          // Query or response                                    [1]
        public int OPCODE;      // What type of query                                   [4]
        public int AA;          // Authoritative Answer (Response-only)                 [1]
        public int TC;          // Truncation Desired.                                  [1]
        public int RD;          // Recursion Desired. (Copied to response)              [1]
        public int RA;          // Recursion Available (Response-only)                  [1]
        public const int Z = 0; // Reserved for future use                              [3]
        public int RCODE;       // Response code.                                       [4]

        #endregion

        public short QDCOUNT;      // Number of question entries                       [16]
        public short ANCOUNT;      // Number of RRs in the answer section.             [16]
        public short NSCOUNT;      // Number of Name Server RRs in the AR section      [16]
        public short ARCOUNT;      // NUmber of RRs in the additional records section  [16]
    }

    public class QuestionSection
    {
        byte[] QNAME;   // Length octet followed by that number of octets. Terminates with the zero length octet for the null label of the root. May be odd number length
        short QTYPE;      // Two octet code specifyign the type of the query. Type field. 
        short QCLASS;     // Two octet code that specifies the class of the query. QCLASS labels.
    }


    public class ResourceRecord
    {
        // Answer, authority, and additional sections all share the same format.
        // Number located in the header.

        byte[] NAME;                // Domain name to which this resource record pertains.
        short TYPE;                 // Two octet containing one of the RR type codes. 
        short CLASS;                // Two octets specifying the class of the data in RDATA field.
        int TTL;                    // Time to live. 
        short RDLENGTH;             // Length of the RDATA field.
        byte[] resource;            // Variable length string of octets.



    }


}
