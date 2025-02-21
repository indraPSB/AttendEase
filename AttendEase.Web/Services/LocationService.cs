using AttendEase.Shared.Services;
using Microsoft.JSInterop;

namespace AttendEase.Web.Services;

public class LocationService(IJSRuntime jSRuntime) : ILocationService
{
    private readonly IJSRuntime _jSRuntime = jSRuntime;

    public async Task<Location> GetCurrentLocation()
    {
        await Task.Yield();
        return new Location(0, 0);
    }
}
