using Helpers.NetworkDiscovery;
using Helpers.NetworkDiscovery.Concrete;
using Helpers.TPLink;
using Microsoft.Extensions.DependencyInjection;
using TPLinkRelayApi.Services.Concrete;

namespace TPLinkRelayApi.Services.Tests;

public sealed class Fixture : IDisposable, IAsyncDisposable
{
	private readonly ServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddMemoryCache()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddCachingHandler(b => b.Expiration = TimeSpan.FromHours(1))
			.AddHttpClient<IClient, Client>(name: "NetworkDiscoveryClient", c => c.BaseAddress = new Uri("https://networkdiscovery/"))
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.Services
			.AddTransient<INetworkDiscoveryService, NetworkDiscoveryService>()
			.AddTPLink()
			.AddTransient<IDeviceService, DeviceService>()
			.BuildServiceProvider();

		DeviceService = _serviceProvider.GetRequiredService<IDeviceService>();
		NetworkDiscoveryClient = _serviceProvider.GetRequiredService<IClient>();
		NetworkDiscoveryService = _serviceProvider.GetRequiredService<INetworkDiscoveryService>();
		TPLinkService = _serviceProvider.GetRequiredService<IService>();
	}

	public IDeviceService DeviceService { get; }
	public IClient NetworkDiscoveryClient { get; }
	public INetworkDiscoveryService NetworkDiscoveryService { get; }
	public IService TPLinkService { get; }

	public void Dispose() => _serviceProvider.Dispose();
	public ValueTask DisposeAsync() => _serviceProvider.DisposeAsync();
}
