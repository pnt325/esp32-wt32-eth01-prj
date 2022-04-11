using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class Invalid : IData
    {
        const int length = 0;
        public byte[] GetBytes()
        {
            return null;
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
