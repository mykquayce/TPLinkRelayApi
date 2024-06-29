using System.Net;
using System.Net.NetworkInformation;
using TPLinkRelayApi.Models;

namespace TPLinkRelayApi.Services;

public interface IDeviceService
{
	Task<Device> GetDeviceAsync(IPAddress ip, CancellationToken cancellationToken = default);
	Task<Device> GetDeviceAsync(PhysicalAddress mac, CancellationToken cancellationToken = default);
	Task<Device> GetDeviceAsync(string alias, CancellationToken cancellationToken = default);
}
