using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GridEye.Protocol;
using System.Diagnostics;

namespace GridEye.ViewModels
{
    public class MainPageViewModel :ViewModelBase
    {
        private bool showProcess;
        private string _wifi_ssid;
        private string _wifi_password;
        private string _mqtt_host;
        private string _mqtt_port;
        private string _ca_data;
        private string _work_hour_1;
        private string _work_hour_2;
        private string _work_hour_3;
        private string _ntc_temp_1;
        private string _ntc_temp_2;
        private string _ntc_temp_3;
        private string _ntc_temp_offset;
        private string _di_input_1;
        private string _di_input_2;
        private string _di_input_3;
        private string _temp_limit;
        private string _device_id;

        //public int Connection { get => _connection; set => SetProperty(ref _connection, value);  }
        public string WifiSsid { get => _wifi_ssid; set => SetProperty(ref _wifi_ssid, value); }
        public string WifiPassword { get => _wifi_password; set => SetProperty(ref _wifi_password, value); }
        public string MqttHost { get => _mqtt_host; set => SetProperty(ref _mqtt_host, value); }
        public string MqttPort { get => _mqtt_port; set => SetProperty(ref _mqtt_port, value); }
        public string CaData { get => _ca_data; set => SetProperty(ref _ca_data, value); }
        public string WorkHour1 { get => _work_hour_1; set => SetProperty(ref _work_hour_1, value); }
        public string WorkHour2 { get => _work_hour_2; set => SetProperty(ref _work_hour_2, value); }
        public string WorkHour3 { get => _work_hour_3; set => SetProperty(ref _work_hour_3, value); }
        public string NtcTempOffset { get => _ntc_temp_offset; set => SetProperty(ref _ntc_temp_offset, value); }
        public string NtcTemp1 { get => _ntc_temp_1; set => SetProperty(ref _ntc_temp_1, value); }
        public string NtcTemp2 { get => _ntc_temp_2; set => SetProperty(ref _ntc_temp_2, value); }
        public string NtcTemp3 { get => _ntc_temp_3; set => SetProperty(ref _ntc_temp_3, value); }
        public string DiInput1 { get => _di_input_1; set => SetProperty(ref _di_input_1, value); }
        public string DiInput2 { get => _di_input_2; set => SetProperty(ref _di_input_2, value); }
        public string DiInput3 { get => _di_input_3; set => SetProperty(ref _di_input_3, value); }
        public string TempLimit { get => _temp_limit; set => SetProperty(ref _temp_limit, value); }
        public string DeviceId { get => _device_id; set => SetProperty(ref _device_id, value); }
        public bool ShowProcess { get => showProcess; set => SetProperty(ref showProcess, value); }
        
        public MainPageViewModel()
        {
        }
    }
}
