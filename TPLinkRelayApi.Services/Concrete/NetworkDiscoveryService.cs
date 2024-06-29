using Helpers.NetworkDiscovery;
using Helpers.Networking.Models;
using System.Runtime.CompilerServices;

namespace TPLinkRelayApi.Services.Concrete;

public class NetworkDiscoveryService(IClient client) : INetworkDiscoveryService
{
	private static readonly byte[] _manufacturerMacAddressBytes = [0x00, 0x31, 0x92,];

	public async IAsyncEnumerable<DhcpLease> GetAllTPLinkDevicesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await foreach (var lease in client.GetAllLeasesAsync(cancellationToken))
		{
			var bytes = lease.PhysicalAddress.GetAddressBytes();

			if (bytes[..3].SequenceEqual(_manufacturerMacAddressBytes))
			{
				yield return lease;
			}
		}
	}
}
