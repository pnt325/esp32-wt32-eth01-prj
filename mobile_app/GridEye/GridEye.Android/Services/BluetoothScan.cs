using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Runtime;
using GridEye.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(GridEye.Droid.Services.BluetoothScan))]
namespace GridEye.Droid.Services
{
    public class BluetoothScan : ScanCallback, GridEye.Services.IBluetoothScan
    {
        bool scanning = false;
        BluetoothLeScanner leScaner;
        ConcurrentQueue<BluetoothScanEventArgs> scanQueue = new ConcurrentQueue<BluetoothScanEventArgs>();
        List<BluetoothScanEventArgs> scanlist = new List<BluetoothScanEventArgs>();
        Thread thScan;
        //List<ScanFilter> filters = new List<ScanFilter>() { new ScanFilter.Builder().SetManufacturerData(0x4652, new byte[] { 0x57, 0x49 }).Build() };
        List<ScanFilter> filters = new List<ScanFilter>() { new ScanFilter.Builder().SetDeviceName("WT32-ETH01").Build() };
        ScanSettings scanSettings = new ScanSettings.Builder()
            .SetScanMode(Android.Bluetooth.LE.ScanMode.LowLatency)
            .SetCallbackType(ScanCallbackType.AllMatches)
            .SetMatchMode(BluetoothScanMatchMode.Aggressive)
            .SetNumOfMatches(1)
            .SetReportDelay(0).Build();

        public event BluetoothScanReceivedEventHandler BluetoothScanReceived;

        public bool Scanning()
        {
            return scanning;
        }

        public bool Start()
        {
            if (scanning)
            {
                return false;
            }

            var bluetoothManager = Android.App.Application.Context.GetSystemService(Context.BluetoothService) as BluetoothManager;
            BluetoothAdapter adapter = bluetoothManager.Adapter;
            leScaner = adapter.BluetoothLeScanner;
            if (leScaner == null)
            {
                return false;
            }

            System.Diagnostics.Debug.WriteLine("Started", "BLE_SCAN");
            scanlist.Clear();
            leScaner.StartScan(filters, scanSettings, this);
            scanning = true;
            thScan = new Thread(() =>
            {
                BluetoothScanEventArgs arg;
                bool addNew = true;
                long rxTimestampMillis;
                long updateMs = Android.OS.SystemClock.ElapsedRealtime();
                while (scanning)
                {
                    if (scanQueue.TryDequeue(out arg))
                    {
                        addNew = true;
                        for (int i = 0; i < scanlist.Count; i++)
                        {
                            if (scanlist[i].Address == arg.Address)
                            {
                                scanlist[i] = arg;
                                addNew = false;
                                //scanlist[i].Report = BluetoothScanReport.Update;
                                //BluetoothScanReceived?.Invoke(this, scanlist[i]);
                                break;
                            }
                        }

                        if (addNew)
                        {
                            scanlist.Add(arg);
                            arg.Report = BluetoothScanReport.Add;
                            BluetoothScanReceived?.Invoke(this, arg);
                        }
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }

                    // Check to remove 
                    for (int i = 0; i < scanlist.Count; i++)
                    {
                        rxTimestampMillis = Android.OS.SystemClock.ElapsedRealtime() - scanlist[i].TimeStampMilis;
                        if (rxTimestampMillis >= 5000)
                        {
                            scanlist[i].Report = BluetoothScanReport.Remove;
                            BluetoothScanReceived?.Invoke(this, scanlist[i]);
                            scanlist.RemoveAt(i);
                            //System.Diagnostics.Debug.WriteLine("Removed", "BLE_SCAN");
                            i--;
                        }
                    }

                    // each 1000 ms
                    if (scanlist.Count != 0)
                    {
                        if (Android.OS.SystemClock.ElapsedRealtime() - updateMs >= 1000)
                        {
                            for (int i = 0; i < scanlist.Count; i++)
                            {
                                scanlist[i].Report = BluetoothScanReport.Update;
                                BluetoothScanReceived?.Invoke(this, scanlist[i]);
                            }
                            updateMs = Android.OS.SystemClock.ElapsedRealtime();
                        }
                    }
                }
            });
            thScan.IsBackground = true;
            thScan.Start();
            return true;
        }

        public void Stop()
        {
            if (!scanning)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine("Stopped", "BLE_SCAN");
            leScaner.StopScan(this);
            scanning = false;
            try
            {
                thScan.Abort();
            }
            catch
            {
            }
        }

        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            scanQueue.Enqueue(new BluetoothScanEventArgs() { Address = result.Device.Address, Name = result.Device.Name, Rssi = result.Rssi, TimeStampMilis = result.TimestampNanos / 1000000 });
            //System.Diagnostics.Debug.WriteLine($"{result.Device.Name}, {result.TimestampNanos/1000000}", "BLE_SCAN");
        }
    }
}