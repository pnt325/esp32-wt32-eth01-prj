using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class SmokeSync : IData
    {
        const int _length = 1;
        public bool Sync { get; set; }
        public byte[] GetBytes()
        {
            byte[] b = new byte[_length];
            b[0] = (byte)(Sync ? 0x01 : 0x00);
            return b;
        }

        public int GetLength()
        {
            return _length;
        }

        public bool Parse(byte[] data, int length)
        {
            if(length < _length)
            {
                return false;
            }
            Sync = data[0] != 0;
            return true;
        }
    }
}
