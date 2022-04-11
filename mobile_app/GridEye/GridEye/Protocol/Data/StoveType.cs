using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class StoveType : IData
    {
        const int _length = 1;
        public State.StoveType Type { get; set; }
        public byte[] GetBytes()
        {
            byte[] b = new byte[_length];
            b[0] = (byte)Type;
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
            Type = (State.StoveType)data[0];
            return true;
        }
    }
}
