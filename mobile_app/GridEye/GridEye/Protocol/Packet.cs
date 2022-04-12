using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol
{
    public class Packet
    {
        public Command Cmd { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }
    }
}
