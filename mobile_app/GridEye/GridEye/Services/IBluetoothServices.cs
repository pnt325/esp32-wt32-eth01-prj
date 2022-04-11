using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Services
{
    public interface IBluetoothServices
    {
        bool Enabled();
        void OpenSetting();
    }
}
