using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WT32EHT01.Models
{
    public class DeviceScanModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private int rssi;
        private string address;

        public string Name
        {
            get => name;
            set
            {
                if(name != value)
                {
                    SetProperty(ref name, value);
                }
            }
        }

        public int Rssi
        {
            get => rssi;
            set
            {
                if(rssi != value)
                {
                    SetProperty(ref rssi, value);
                }
            }
        }

        public string Address { get => address; set => address = value; }

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