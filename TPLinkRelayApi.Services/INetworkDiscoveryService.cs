using Helpers.Networking.Models;
using System.Runtime.CompilerServices;

namespace TPLinkRelayApi.Services;
public interface INetworkDiscoveryService
{
	IAsyncEnumerable<DhcpLease> GetAllTPLinkDevicesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
}
