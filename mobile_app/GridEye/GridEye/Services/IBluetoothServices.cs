using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Services
{
    public interface IBluetoothServices
    {
        bool Enabled();
        void OpenSetting();
    }
}
