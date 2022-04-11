using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GridEye
{
    interface RequestGetPermission
    {
        Task<PermissionStatus> GetAsync();
        Task<PermissionStatus> RequesAsync();
    }
}
