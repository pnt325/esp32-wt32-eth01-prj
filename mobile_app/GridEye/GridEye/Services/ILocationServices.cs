using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace WT32EHT01.Services
{
    public interface ILocationServices
    {
        bool Enabled();
        void OpenSetting();
    }
}
