using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WT32EHT01.Protocol
{
    public class Protocol
    {
        public const int DATA_SIZE_MIN = 3;
        public const int DATA_SIZE_MAX = 128;
        public event ProtocolReceivedEventHandler Received;

        public Protocol(Services.IBluetoothGatt gatt)
        {
            gatt.Received += Gatt_Received;
        }

        public bool Send(Command cmd, byte[] data, int length)
        {
            if ((data == null && length != 0) || (data != null && length > data.Length) || length > DATA_SIZE_MAX)
            {
                return false;
            }

            byte[] sendBuf = new byte[length + DATA_SIZE_MIN];

            byte cksum = 0xff;
            int index = 0;
            sendBuf[index++] = (byte)cmd;
            sendBuf[index++] = (byte)length;

            cksum = (byte)(sendBuf[0] + cksum);
            cksum = (byte)(sendBuf[1] + cksum);

            for (int i = 0; i < length; i++)
            {
                sendBuf[index++] = data[i];
                cksum = (byte)(cksum + data[i]);
            }

            sendBuf[index++] = cksum;
            return App.BluetoothGatt.Write(sendBuf);
        }

        private void Parse(byte[] data)
        {
            // [cmd][len][cksum]
            if (data.Length < DATA_SIZE_MIN)
            {
                return;
            }

            Packet packet = new Packet();
            packet.Cmd = (Command)data[0];
            packet.Length = data[1];

            Debug.WriteLine($"CMD: {packet.Cmd}, LEN: {packet.Length}");

            if(packet.Length != 0)
            {
                packet.Data = new byte[packet.Length];
            }

            int i;
            for (i = 0; i < packet.Length; i++)
            {
                packet.Data[i] = data[i + 2];
            }

            byte sum = 0xff;
            for(i = 0; i < packet.Length + 2; i++)
            {
                sum = (byte)(sum + data[i]);
            }

            if(sum != data[i])
            {
                return;
            }

            Received?.Invoke(this, packet);
        }

        private void Gatt_Received(object sender, byte[] data)
        {
            this.Parse(data);
        }
    }
}
