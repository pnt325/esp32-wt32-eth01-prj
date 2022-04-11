namespace GridEye.Protocol.Data
{
    public class CookArea : IData
    {
        const int _length = 6;
        public int MinRow { get; set; }
        public int MinCol { get; set; }
        public int MaxRow { get; set; }
        public int MaxCol { get; set; }
        public int BeginRow { get; set; }
        public int EndRow { get; set; }

        public byte[] GetBytes()
        {
            byte[] b = new byte[_length];
            b[0] = (byte)MinRow;
            b[1] = (byte)MinCol;
            b[2] = (byte)MaxRow;
            b[3] = (byte)MaxCol;
            b[4] = (byte)BeginRow;
            b[5] = (byte)EndRow;
            return b;
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

            MinRow = data[0];
            MinCol = data[1];
            MaxRow = data[2];
            MaxCol = data[3];
            BeginRow = data[4];
            EndRow = data[5];

            return true;
        }
    }
}
