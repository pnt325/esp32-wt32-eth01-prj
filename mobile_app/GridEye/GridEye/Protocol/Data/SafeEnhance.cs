using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class SafeEnhance : IData
    {
        const int _length = 1;
        public bool Enable { get; set; }
        public byte[] GetBytes()
        {
            byte[] b = new byte[_length];
            b[0] = (byte)(Enable ? 1 : 0);
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

            Enable = data[0] != 0;

            return true;
        }
    }
}
