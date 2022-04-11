using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Services
{
    public class BluetoothScanEventArgs : EventArgs
    {
        public string Name { get; set; }
        public int Rssi { get; set; }
        public string Address { get; set; }
        public BluetoothScanReport Report { get; set; }
        public long TimeStampMilis { get; set; }
    }

    public delegate void BluetoothScanReceivedEventHandler(object sender, BluetoothScanEventArgs args);
}
