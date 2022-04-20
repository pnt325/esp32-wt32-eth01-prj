using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Protocol.Data
{
    public interface IData
    {
        bool Parse(byte[] data, int length);
        byte[] GetBytes();
        int GetLength();
    }
}
