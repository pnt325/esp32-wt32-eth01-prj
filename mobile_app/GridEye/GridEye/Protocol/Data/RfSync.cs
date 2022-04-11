using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class RfSync : IData
    {
        const int length = 9;
        public byte[] MainSw { get; set; }
        public byte[] SmokeSw { get; set; }
        public State.SyncMode Mode { get; set; }

        public RfSync()
        {
            MainSw = new byte[4];
            SmokeSw = new byte[4];
        }

        public byte[] GetBytes()
        {
            byte[] b = new byte[length];
            b[0] = MainSw[0];
            b[1] = MainSw[1];
            b[2] = MainSw[2];
            b[3] = MainSw[3];
            b[4] = SmokeSw[0];
            b[5] = SmokeSw[1];
            b[6] = SmokeSw[2];
            b[7] = SmokeSw[3];
            b[7] = (byte)Mode;
            return b;
        }

        public int GetLength()
        {
            return length;
        }

        public bool Parse(byte[] data, int length)
        {
            return true;
        }
    }
}
