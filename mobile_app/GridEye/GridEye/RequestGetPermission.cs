using System.Threading.Tasks;
using Xamarin.Essentials;

namespace WT32EHT01
{
    interface RequestGetPermission
    {
        Task<PermissionStatus> GetAsync();
        Task<PermissionStatus> RequesAsync();
    }
}
