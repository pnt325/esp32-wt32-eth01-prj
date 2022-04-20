using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Protocol.Data
{
    public class Digital : IData
    {

        const int len = 3;

        public bool[] Values;


        public Digital()
        {
            Values = new bool[3];
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
            if(data.Length < len || length < len)
            {
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                Values[i] = data[i] != 0 ? true : false;
            }

            return true;
        }
    }
}
