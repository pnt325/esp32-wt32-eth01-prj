using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class HumanDetect : IData
    {
        const int _length = 17;
        public int Count { get; set; }
        public int[] Rows { get; set; }
        public int[] Cols { get; set; }
        public int[] NRows { get; set; }
        public int[] NCols { get; set; }

        public HumanDetect()
        {
            Rows = new int[4];
            Cols = new int[4];
            NRows = new int[4];
            NCols = new int[4];
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

            Count = data[0];
            Rows[0] = data[1];
            Rows[1] = data[2];
            Rows[2] = data[3];
            Rows[3] = data[4];

            Cols[0] = data[5];
            Cols[1] = data[6];
            Cols[2] = data[7];
            Cols[3] = data[8];

            NRows[0] = data[9];
            NRows[1] = data[10];
            NRows[2] = data[11];
            NRows[3] = data[12];

            NCols[0] = data[13];
            NCols[1] = data[14];
            NCols[2] = data[15];
            NCols[3] = data[16];

            return true;
        }
    }
}
