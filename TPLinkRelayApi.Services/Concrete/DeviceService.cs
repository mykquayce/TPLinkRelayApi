using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using TPLinkRelayApi.Models;

namespace TPLinkRelayApi.Services.Concrete;

public class DeviceService(
	IMemoryCache memoryCache,
	INetworkDiscoveryService networkDiscoveryService,
	Helpers.TPLink.IService tpLinkService) : IDeviceService
{
	private const string _keyPrefix = "device:";
	private static readonly TimeSpan _absoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

	public Task<Device> GetDeviceAsync(string alias, CancellationToken cancellationToken = default) => TryGetDeviceAsync(alias, cancellationToken);
	public Task<Device> GetDeviceAsync(IPAddress ip, CancellationToken cancellationToken = default) => TryGetDeviceAsync(ip, cancellationToken);
	public Task<Device> GetDeviceAsync(PhysicalAddress mac, CancellationToken cancellationToken = default) => TryGetDeviceAsync(mac, cancellationToken);

	private async Task<Device> TryGetDeviceAsync(object key, CancellationToken cancellationToken = default)
	{
		if (memoryCache.TryGetValue<Device>(_keyPrefix + key, out var device))
		{
			return device;
		}

		await PopulateCacheAsync(cancellationToken);

		if (memoryCache.TryGetValue(_keyPrefix + key, out device))
		{
			return device;
		}

		throw new KeyNotFoundException("key not found in cache: " + key);
	}

	private async Task PopulateCacheAsync(CancellationToken cancellationToken = default)
	{
		await foreach (var device in GetDevicesAsync(cancellationToken))
		{
			f(device.Alias);
			f(device.IPAddress);
			f(device.PhysicalAddress);

			void f(object key) => memoryCache.Set(_keyPrefix + key, device, _absoluteExpirationRelativeToNow);
		}
	}

	private async IAsyncEnumerable<Device> GetDevicesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var dhcps = networkDiscoveryService.GetAllTPLinkDevicesAsync(cancellationToken);

		await foreach (var dhcp in dhcps)
		{
			var info = await tpLinkService.GetSystemInfoAsync(dhcp.IPAddress, cancellationToken);
			yield return new(info.alias, dhcp.IPAddress, dhcp.PhysicalAddress);
		}
	}
}
