using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Protocol.Data
{
    public class Temperature:IData
    {
        const int len = 12;
        public float[] Values;

        public Temperature()
        {
            Values = new float[3];
        }

        public byte[] GetBytes()
        {
            return null;
        }

        public int GetLength()
        {
            return len;
        }

        public bool Parse(byte[] data, int length)
        {
            if(data.Length < length ||  length < len)
            {
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                Values[i] = (float)Math.Round(BitConverter.ToSingle(data, i * 4), 2);
            }

            return true;
        }
    }
}
