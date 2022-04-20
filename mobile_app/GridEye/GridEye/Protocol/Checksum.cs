namespace WT32EHT01.Protocol
{
    class Checksum
    {
        const byte CRC_INIT = 0x4F;
        const byte POLY_INIT = 0x8C;
        public static byte Calc(byte[] data, int startIndex, int length)
        {
            byte crc = CRC_INIT;
            int i;
            int j;
            for (i = 0; i < length; i++)
            {
                crc ^= data[i + startIndex];
                for (j = 0; j < 8; j++)
                {
                    if ((crc & 0x01) != 0)
                    {
                        crc >>= 1;
                        crc ^= POLY_INIT;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }
    }
}
