using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WT32EHT01.Protocol;
using System.Diagnostics;

namespace WT32EHT01.ViewModels
{
    public class MainPageViewModel :ViewModelBase
    {
        private bool showProcess;
        private string _wifi_ssid;
        private string _wifi_password;
        private string _work_hour_1;
        private string _work_hour_2;
        private string _work_hour_3;
        private string _ntc_temp_1;
        private string _ntc_temp_2;
        private string _ntc_temp_3;
        private string _ntc_temp_offset_1;
        private string _ntc_temp_offset_2;
        private string _ntc_temp_offset_3;
        private string _di_input_1;
        private string _di_input_2;
        private string _di_input_3;
        private string _temp_limit_1;
        private string _temp_limit_2;
        private string _temp_limit_3;
        private string _device_id;
        private string _device_token_1;
        private string _device_token_2;
        private string _device_token_3;
        private bool _device_enable_1;
        private bool _device_enable_2;
        private bool _device_enable_3;

        //public int Connection { get => _connection; set => SetProperty(ref _connection, value);  }
        public string WifiSsid { get => _wifi_ssid; set => SetProperty(ref _wifi_ssid, value); }
        public string WifiPassword { get => _wifi_password; set => SetProperty(ref _wifi_password, value); }
        public string WorkHour1 { get => _work_hour_1; set => SetProperty(ref _work_hour_1, value); }
        public string WorkHour2 { get => _work_hour_2; set => SetProperty(ref _work_hour_2, value); }
        public string WorkHour3 { get => _work_hour_3; set => SetProperty(ref _work_hour_3, value); }
        public string NtcTempOffset1 { get => _ntc_temp_offset_1; set => SetProperty(ref _ntc_temp_offset_1, value); }
        public string NtcTempOffset2 { get => _ntc_temp_offset_2; set => SetProperty(ref _ntc_temp_offset_2, value); }
        public string NtcTempOffset3 { get => _ntc_temp_offset_3; set => SetProperty(ref _ntc_temp_offset_3, value); }
        public string NtcTemp1 { get => _ntc_temp_1; set => SetProperty(ref _ntc_temp_1, value); }
        public string NtcTemp2 { get => _ntc_temp_2; set => SetProperty(ref _ntc_temp_2, value); }
        public string NtcTemp3 { get => _ntc_temp_3; set => SetProperty(ref _ntc_temp_3, value); }
        public string DiInput1 { get => _di_input_1; set => SetProperty(ref _di_input_1, value); }
        public string DiInput2 { get => _di_input_2; set => SetProperty(ref _di_input_2, value); }
        public string DiInput3 { get => _di_input_3; set => SetProperty(ref _di_input_3, value); }
        public string TempLimit1 { get => _temp_limit_1; set => SetProperty(ref _temp_limit_1, value); }
        public string TempLimit2 { get => _temp_limit_2; set => SetProperty(ref _temp_limit_2, value); }
        public string TempLimit3 { get => _temp_limit_3; set => SetProperty(ref _temp_limit_3, value); }
        public string DeviceId { get => _device_id; set => SetProperty(ref _device_id, value); }
        public string DeviceToken1 { get => _device_token_1; set => SetProperty(ref _device_token_1, value); }
        public string DeviceToken2 { get => _device_token_2; set => SetProperty(ref _device_token_2, value); }
        public string DeviceToken3 { get => _device_token_3; set => SetProperty(ref _device_token_3, value); }
        public bool DeviceEnable1 { get => _device_enable_1; set => SetProperty(ref _device_enable_1, value); }
        public bool DeviceEnable2 { get => _device_enable_2; set => SetProperty(ref _device_enable_2, value); }
        public bool DeviceEnable3 { get => _device_enable_3; set => SetProperty(ref _device_enable_3, value); }
        public bool ShowProcess { get => showProcess; set => SetProperty(ref showProcess, value); }
        
        public MainPageViewModel()
        {
        }
    }
}
