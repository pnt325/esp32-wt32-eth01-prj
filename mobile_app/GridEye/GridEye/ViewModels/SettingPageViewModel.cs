using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GridEye.ViewModels
{
    public class SettingPageViewModel:INotifyPropertyChanged
    {
        int battery;
        bool safeEnhance;
        bool stoveOven;
        string cookerType;
        string model;
        string firmwareVersion;
        string serialNumber;
        bool overlayOkEnable;
        bool showCookerOverlay;
        bool showSafeEnhanceOverlay;

        public int Battery { get => battery; set { if (battery != value) SetProperty(ref battery, value); } }

        public bool SafeEnhance { get => safeEnhance; set { if (safeEnhance != value) SetProperty(ref safeEnhance, value); } }

        public bool StoveOven { get => stoveOven; set { if (stoveOven != value) SetProperty(ref stoveOven, value); } }

        public string CookerType { get => cookerType; set { if (cookerType != value) SetProperty(ref cookerType, value); } }

        public string Model { get => model; set { if (model != value) SetProperty(ref model, value); } }

        public string FirmwareVersion { get => firmwareVersion; set { if (firmwareVersion != value) SetProperty(ref firmwareVersion, value); } }

        public string SerialNumber { get => serialNumber; set { if (serialNumber != value) SetProperty(ref serialNumber, value); } }

        public bool OverlayOkEnable { get => overlayOkEnable; set { if (overlayOkEnable != value) SetProperty(ref overlayOkEnable, value); } }

        public bool ShowCookerOverlay { get => showCookerOverlay; set { if (showCookerOverlay != value) SetProperty(ref showCookerOverlay, value); } }

        public bool ShowSafeEnhanceOverlay { get => showSafeEnhanceOverlay; set { if (showSafeEnhanceOverlay != value) SetProperty(ref showSafeEnhanceOverlay, value); } }

        public SettingPageViewModel()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
