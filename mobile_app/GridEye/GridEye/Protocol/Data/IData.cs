using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public interface IData
    {
        bool Parse(byte[] data, int length);
        byte[] GetBytes();
        int GetLength();
    }
}
