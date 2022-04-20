using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Services
{
    public interface IBluetoothGatt
    {
        event BluetoothGattReceivedEventHandler Received;
        event EventHandler Disconnected;
        event BluetoothGattConnectedEventHandler Connected;
        void Connect(string address);
        void Disconnect();
        bool Write(byte[] datas);
        bool IsConnected();
    }

    public delegate void BluetoothGattConnectedEventHandler(bool result);
}
