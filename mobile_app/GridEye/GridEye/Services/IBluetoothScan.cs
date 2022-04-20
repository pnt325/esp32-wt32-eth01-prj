using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Services
{
    public interface IBluetoothScan
    {
        event BluetoothScanReceivedEventHandler BluetoothScanReceived;
        bool Scanning();
        bool Start();
        void Stop();
    }
}
