using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GridEye.Services
{
    public interface IBLEPermission
    {
        void Request();
        bool Result();
    }
}
