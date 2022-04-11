using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class MainInfo : IData
    {
        const int _length = 8;
        public int Version { get; set; }
        public int SmokeLastSyncPeriod { get; set; }
        public float Current { get; set; }
        public bool Lock { get; set; }

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

            Version = BitConverter.ToUInt16(data, 0);
            SmokeLastSyncPeriod = BitConverter.ToUInt16(data, 2);
            Current = (float)BitConverter.ToUInt16(data, 4) / 10.0f;
            Lock = data[7] != 0;
            return true;
        }
    }
}
