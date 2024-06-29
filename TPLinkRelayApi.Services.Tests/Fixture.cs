using Helpers.NetworkDiscovery;
using Helpers.NetworkDiscovery.Concrete;
using Microsoft.Extensions.DependencyInjection;
using TPLinkRelayApi.Services.Concrete;

namespace TPLinkRelayApi.Services.Tests;

public sealed class Fixture : IDisposable, IAsyncDisposable
{
	private readonly ServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddHttpClient<IClient, Client>(name: "NetworkDiscoveryClient", c => c.BaseAddress = new Uri("https://networkdiscovery/"))
				.Services
			.AddTransient<INetworkDiscoveryService, NetworkDiscoveryService>()
			.BuildServiceProvider();

		NetworkDiscoveryClient = _serviceProvider.GetRequiredService<IClient>();
		NetworkDiscoveryService = _serviceProvider.GetRequiredService<INetworkDiscoveryService>();
	}

	public IClient NetworkDiscoveryClient { get; }
	public INetworkDiscoveryService NetworkDiscoveryService { get; }

	public void Dispose() => _serviceProvider.Dispose();
	public ValueTask DisposeAsync() => _serviceProvider.DisposeAsync();
}
