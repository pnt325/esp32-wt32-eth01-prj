using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Protocol
{
    public class Packet
    {
        public Command Cmd { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }
    }
}
