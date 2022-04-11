using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GridEye.ViewModels
{
    public class InstallSyncPageViewModel : INotifyPropertyChanged
    {
        bool showProcess;
        bool syncTypeOverlay;
        bool overlayOkEnable;
        bool smokeSwVisible;
        bool mainSwVisible;
        bool cameraOvelay;
        string scanResult;
        string syncTypeResult;
        bool nextEnable;

        public bool ShowProcess { get => showProcess; set => SetProperty(ref showProcess, value); }
        public bool SyncTypeOverlay { get => syncTypeOverlay; set => SetProperty(ref syncTypeOverlay, value); }
        public bool OverlayOkEnable { get => overlayOkEnable; set => SetProperty(ref overlayOkEnable, value); }
        public bool CameraOvelay { get => cameraOvelay; set => SetProperty(ref cameraOvelay, value); }
        public string ScanResult { get => scanResult; set { if (scanResult != value) SetProperty(ref scanResult, value); } }

        public string SyncTypeResult { get => syncTypeResult; set { if (syncTypeResult != value) SetProperty(ref syncTypeResult, value); } }

        public bool NextEnable { get => nextEnable; set => SetProperty(ref nextEnable, value); }
        public bool SmokeSwVisible { get => smokeSwVisible; set => SetProperty(ref smokeSwVisible, value); }
        public bool MainSwVisible { get => mainSwVisible; set => SetProperty(ref mainSwVisible, value); }

        public InstallSyncPageViewModel()
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
