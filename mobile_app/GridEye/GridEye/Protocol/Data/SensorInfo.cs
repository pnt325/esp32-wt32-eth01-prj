using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol.Data
{
    public class SensorInfo : IData
    {
        const int _length = 14;
        const int _snLength = 12;
        public int BatteryCapacity { get; set; }
        public State.SafeProfile SafeProfile { get; set; }
        public bool SafeEnhance { get; set; }
        public State.AppMode Mode { get; set; }
        public int BatteryVolt { get; set; }
        public int Version { get; set; }
        public State.StoveType StoveType { get; set; }
        public State.TaskState TaskState { get; set; }
        public int Temperature { get; set; }
        public bool CookAreadConfig { get; set; }
        public State.SyncMode SyncMode { get; set; }

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

            BatteryCapacity = data[0];
            SafeProfile = (State.SafeProfile)data[1];
            SafeEnhance = data[2] != 0;
            Mode = (State.AppMode)data[3];
            BatteryVolt = BitConverter.ToUInt16(data, 4);
            Version = BitConverter.ToUInt16(data, 6);
            StoveType = (State.StoveType)data[8];
            TaskState = (State.TaskState)data[9];
            Temperature = BitConverter.ToUInt16(data, 10);
            CookAreadConfig = data[12] != 0;
            SyncMode = (State.SyncMode)data[13];
            return true;
        }
    }
}
