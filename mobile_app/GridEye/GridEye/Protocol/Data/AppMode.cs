using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class AppMode : IData
    {
        const int _length = 1;
        public State.AppMode Mode { get; set; }
        public byte[] GetBytes()
        {
            byte[] b = new byte[_length];
            b[0] = (byte)Mode;
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
            Mode = (State.AppMode)data[0];
            return true;
        }
    }
}
