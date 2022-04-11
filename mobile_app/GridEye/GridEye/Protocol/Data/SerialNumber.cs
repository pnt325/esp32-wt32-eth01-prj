using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class SerialNumber : IData
    {
        const int _length = 32;
        public string Sn;
        public byte[] GetBytes()
        {
            return null;
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

            Sn = BitConverter.ToString(data, 0, _length);
            return true;
        }
    }
}
