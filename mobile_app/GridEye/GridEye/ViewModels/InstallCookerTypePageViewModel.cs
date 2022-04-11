using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GridEye.ViewModels
{
    public class InstallCookerTypePageViewModel : INotifyPropertyChanged
    {
        bool typeOverlay;
        bool ovenOverlay;
        string cookerTypeResult;
        string ovenResult;
        bool overlayOkEnable;
        bool showProcess;

        public bool TypeOverlay { get => typeOverlay; set { if (typeOverlay != value) SetProperty(ref typeOverlay, value); } }
        public bool OvenOverlay { get => ovenOverlay; set { if (ovenOverlay != value) SetProperty(ref ovenOverlay, value); } }

        public string CookerTypeResult { get => cookerTypeResult; set { if (cookerTypeResult != value) SetProperty(ref cookerTypeResult, value); } }
        public string OvenResult { get => ovenResult; set { if (ovenResult != value) SetProperty(ref ovenResult, value); } }

        public bool OverlayOkEnable { get => overlayOkEnable; set => SetProperty(ref overlayOkEnable, value); }
        public bool ShowProcess { get => showProcess; set => SetProperty(ref showProcess, value); }

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
