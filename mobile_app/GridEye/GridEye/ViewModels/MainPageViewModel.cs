using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GridEye.Protocol;

namespace GridEye.ViewModels
{
    public class MainPageViewModel :ViewModelBase
    {
        private string system;
        private int battery;
        private string mode;
        private int temperature;
        private string taskState;
        private bool stoveLock;
        private string humanVisible;
        private string safeProfile;
        private bool lockOverlay;
        private bool profileOverlay;
        private string overlayTitle;
        private bool overlayOkEnable;
        private string overlayTitleResult;
        private bool overlayTitleResultVisible;
        private bool lockEnable;
        private bool showProcess;

        // New binding data start ============================== //
        private int _connection;        // 0: None, 1: WIFI, 2: ETH
        private string _mqtt_uri;
        private string _ca_file_path;
        private float _work_hour_1;
        private float _work_hour_2;
        private float _work_hour_3;
        private float _ntc_temp_1;
        private float _ntc_temp_2;
        private float _ntc_temp_3;
        private float _ntc_temp_offset;
        private string _di_input_1;
        private string _di_input_2;
        private string _di_input_3;

        public int Connection { get => _connection; set => SetProperty(ref _connection, value); }
        public string MqttUri { get => _mqtt_uri; set => SetProperty(ref _mqtt_uri, value); }
        public string CaFilePath { get => _ca_file_path; set => SetProperty(ref _ca_file_path, value); }
        public float WorkHour1 { get => _work_hour_1; set => SetProperty(ref _work_hour_1, value); }
        public float WorkHour2 { get => _work_hour_2; set => SetProperty(ref _work_hour_2, value); }
        public float WorkHour3 { get => _work_hour_3; set => SetProperty(ref _work_hour_3, value); }
        public float NtcTempOffset { get => _ntc_temp_offset; set => SetProperty(ref _ntc_temp_offset, value); }
        public float NtcTemp1 { get => _ntc_temp_1; set => SetProperty(ref _ntc_temp_1, value); }
        public float NtcTemp2 { get => _ntc_temp_2; set => SetProperty(ref _ntc_temp_2, value); }
        public float NtcTemp3 { get => _ntc_temp_3; set => SetProperty(ref _ntc_temp_3, value); }
        public string DiInput1 { get => _di_input_1; set => SetProperty(ref _di_input_1, value); }
        public string DiInput2 { get => _di_input_2; set => SetProperty(ref _di_input_2, value); }
        public string DiInput3 { get => _di_input_3; set => SetProperty(ref _di_input_3, value); }
        // New binding data end ================================ //

        public string System { get => system; set {
                if(system != value)
                {
                    SetProperty(ref system, value);
                }
            }
        }

        public int Battery
        {
            get => battery;
            set
            {
                if(battery != value)
                {
                    SetProperty(ref battery, value);
                }
            }
        }

        public string Mode { get => mode; set =>SetProperty(ref mode, value); } 

        public int Temperature { get => temperature; set => SetProperty(ref temperature, value); } 

        public string TaskState { get => taskState; set => SetProperty(ref taskState, value); } 

        public bool StoveLock { get => stoveLock; set => SetProperty(ref stoveLock, value); } 

        public string HumanVisible { get => humanVisible; set => SetProperty(ref humanVisible, value); } 

        public string SafeProfile { get => safeProfile; set => SetProperty(ref safeProfile, value); } 

        public bool LockOverlay { get => lockOverlay; set => SetProperty(ref lockOverlay, value); } 
        public bool ProfileOverlay { get => profileOverlay; set => SetProperty(ref profileOverlay, value); } 

        public string OverlayTitle { get => overlayTitle; set => SetProperty(ref overlayTitle, value); } 

        public bool OverlayOkEnable { get => overlayOkEnable; set => SetProperty(ref overlayOkEnable, value); } 

        public string OverlayTitleResult { get => overlayTitleResult; set => SetProperty(ref overlayTitleResult, value); } 

        public bool OverlayTitleResultVisible { get => overlayTitleResultVisible; set => SetProperty(ref overlayTitleResultVisible, value); } 

        public bool LockEnable { get => lockEnable; set => SetProperty(ref lockEnable, value); } 

        public bool ShowProcess { get => showProcess; set => SetProperty(ref showProcess, value); }

        public MainPageViewModel()
        {

        }
    }
}
