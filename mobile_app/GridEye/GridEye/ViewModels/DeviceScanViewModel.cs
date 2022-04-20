using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WT32EHT01.ViewModels
{
    public class DeviceScanViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Models.DeviceScanModel> _device;

        private bool showGuide = true;
        private bool showProcess = false;

        public DeviceScanViewModel()
        {
            _device = new ObservableCollection<Models.DeviceScanModel>();
        }

        public ObservableCollection<Models.DeviceScanModel> Items
        {
            get { return _device; }
            set { _device = value; }
        }

        public bool ShowGuide
        {
            get => showGuide;
            set
            {
                if(showGuide != value)
                {
                    SetProperty(ref showGuide, value);
                }
            }
        }

        public bool ShowProcess
        {
            get => showProcess;
            set
            {
                if(showProcess != value)
                {
                    SetProperty(ref showProcess, value);
                }
            }
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
