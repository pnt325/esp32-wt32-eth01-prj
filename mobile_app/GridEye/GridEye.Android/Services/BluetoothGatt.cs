using Android.App;
using Android.Bluetooth;
using Android.Runtime;
using GridEye.Services;
using Java.Util;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(GridEye.Droid.Services.BluetoothGatt))]
namespace GridEye.Droid.Services
{
    public delegate void BleGattReceivedEventHandler(byte[] datas);
    public class BluetoothGatt : Android.Bluetooth.BluetoothGattCallback, IBluetoothGatt
    {
        //const string SERVICE_UUID = "69ddd530-8216-480e-a48d-a516ae310fc2";
        //const string CHARACTERISTIC_UUID = "d0962ce0-360e-4db6-a356-b443a20d94e3";

        //EventWaitHandle connectWait;
        Mutex mut = new Mutex();
        bool doConnect = false;
        bool connected = false;

        Android.Bluetooth.BluetoothGatt gatt;
        Android.Bluetooth.BluetoothDevice device;
        Android.Bluetooth.BluetoothGattCharacteristic characteristic;

        public event EventHandler Disconnected;
        public event BluetoothGattReceivedEventHandler Received;
        public event BluetoothGattConnectedEventHandler Connected;

        public void Connect(string address)
        {
            if (doConnect)
            {
                Debug.WriteLine("On Connecting", "BLE_CONN");
                return;
            }
            doConnect = true;

            if (connected)
            {
                doConnect = false;
                return;
            }

            device = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.GetRemoteDevice(address);
            this.gatt = device.ConnectGatt(Application.Context, false, this);
            doConnect = false;
        }

        public void Disconnect()
        {
            connected = false;
            gatt.Disconnect();
        }

        public override void OnCharacteristicChanged(Android.Bluetooth.BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            Received?.Invoke(this, (byte[])characteristic.GetValue().Clone());
        }

        public override void OnServicesDiscovered(Android.Bluetooth.BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
        {
            if (status != GattStatus.Success)
            {
                BleClose(gatt);
                return;
            }

            var service = gatt.GetService(UUID.FromString("000000ff-0000-1000-8000-00805f9b34fb"));
            if (service != null)
            {
                Debug.WriteLine("Get services success", "BLE_SCAN");
                characteristic = service.Characteristics[0];
                if (characteristic == null)
                {
                    Debug.WriteLine("Get characteristic failure", "BLE_SCAN");
                    BleClose(gatt);
                }
                else
                {
                    Debug.WriteLine("Get characteristic success", "BLE_SCAN");
                    if (characteristic.Properties.HasFlag(GattProperty.Notify))
                    {
                        Debug.WriteLine("Get characteristic supported notify", "BLE_SCAN");
                        if (gatt.SetCharacteristicNotification(characteristic, true))
                        {
                            Debug.WriteLine("Get characteristic enable notify success", "BLE_SCAN");
                            UUID uuid = UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");
                            var des = characteristic.GetDescriptor(uuid);
                            des.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
                            if (gatt.WriteDescriptor(des))
                            {
                                Debug.WriteLine("Get characteristic write notify descriptor success", "BLE_SCAN");
                                connected = true;
                                Connected?.Invoke(true);
                            }
                            else
                            {
                                Debug.WriteLine("Get characteristic write notify descriptor failure", "BLE_SCAN");
                                BleClose(gatt);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Get characteristic enable notify failure", "BLE_SCAN");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Get characteristic un-supported notify", "BLE_SCAN");
                        BleClose(gatt);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Get services failure", "BLE_SCAN");
                BleClose(gatt);
            }
        }

        private void BleClose(Android.Bluetooth.BluetoothGatt gatt)
        {
            mut.WaitOne();

            characteristic = null;
            gatt.Close();
            if(connected)
            {
                Disconnected?.Invoke(this, null);
            }
            else
            {
                Connected?.Invoke(false);
            }
            connected = false;
            System.Diagnostics.Debug.WriteLine("Disconnected", "BLE_GATT");

            mut.ReleaseMutex();
        }

        public override void OnConnectionStateChange(Android.Bluetooth.BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
        {
            if (status == GattStatus.Success)
            {
                switch (newState)
                {
                    case ProfileState.Connected:
                        gatt.DiscoverServices();
                        break;
                    case ProfileState.Connecting:
                        break;
                    case ProfileState.Disconnected:
                        Debug.WriteLine("Disconnected", "BLE_GATT");
                        BleClose(gatt);
                        break;
                    case ProfileState.Disconnecting:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                BleClose(gatt);
            }
            //base.OnConnectionStateChange(gatt, status, newState);
        }

        public bool Write(byte[] datas)
        {
            mut.WaitOne();

            bool ret = false;
            //if (gatt.GetConnectionState(this.device) != ProfileState.Connected)
            int retry = 0;
            while(retry > 0)
            {
                if (connected)
                {
                    characteristic.SetValue(datas);
                    characteristic.WriteType = GattWriteType.Default;
                    ret = gatt.WriteCharacteristic(characteristic);

                    if(ret == false)
                    {
                        Debug.WriteLine("BLE: Send retry");
                        retry--;
                    }
                }
                else
                {
                    break;
                }
            }
            if(connected)
            {
                characteristic.SetValue(datas);
                characteristic.WriteType = GattWriteType.Default;
                ret = gatt.WriteCharacteristic(characteristic);
            }

            mut.ReleaseMutex();

            return ret;
        }

        public bool IsConnected()
        {
            return connected;
        }
    }
}