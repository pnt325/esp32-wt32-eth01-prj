using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class StoveLock : IData
    {
        const int _length = 5;
        public bool Lock { get; set; }
        public byte[] Pswd { get; set; }

        public StoveLock()
        {
            Pswd = new byte[4];
        }
        public byte[] GetBytes()
        {
            byte[] b = new byte[_length];
            b[0] = (byte)(Lock ? 1 : 0);
            b[1] = Pswd[0];
            b[2] = Pswd[1];
            b[3] = Pswd[2];
            b[4] = Pswd[3];
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
            Lock = data[0] != 0;
            return true;
        }
    }
}
