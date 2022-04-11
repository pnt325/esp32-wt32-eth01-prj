using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data.Utils
{
    public class VersionHelps
    {
        public static string GetString(int version)
        {
            return $"{version & 0x0000f}.{(version >> 4) & 0x0000f}.{(version >> 8) & 0x0000f}.build-{(version >> 12) & 0x0000f}";
        }
    }
}
