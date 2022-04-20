using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WT32EHT01.Services
{
    public interface IBLEPermission
    {
        void Request();
        bool Result();
    }
}
