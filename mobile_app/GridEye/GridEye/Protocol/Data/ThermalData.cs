using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class ThermalData : IData
    {
        const int _length = 32;
        public int[] Values { get; set; }
        public ThermalData()
        {
            Values = new int[_length];
        }

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
            if (length < _length)
            {
                return false;
            }

            for (int i = 0; i < 16; i += 2)
            {
                Values[i] = BitConverter.ToUInt16(data, i);
            }

            return true;
        }
    }
}
