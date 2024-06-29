using Helpers.TPLink;

namespace TPLinkRelayApi.Services.Tests;

[Collection(nameof(NonParallelCollectionDefinition))]
public class TPLinkServiceTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly INetworkDiscoveryService _networkDiscoveryService = fixture.NetworkDiscoveryService;
	private readonly IService _sut = fixture.TPLinkService;

	[Fact]
	public async Task GetAliasTests()
	{
		// Arrange
		using var cts = new CancellationTokenSource(millisecondsDelay: 3_000);
		var devices = await _networkDiscoveryService.GetAllTPLinkDevicesAsync(cts.Token).ToArrayAsync(cts.Token);

		// Assert
		Assert.NotEmpty(devices);
		Assert.DoesNotContain(default, devices);
		Assert.All(devices, d => Assert.NotEqual(default, d.IPAddress));

		foreach (var device in devices)
		{
			// Act
			var info = await _sut.GetSystemInfoAsync(device.IPAddress, cts.Token);

			// Assert
			Assert.NotEqual(default, info);
			Assert.NotEmpty(info.alias);
		}
	}
}
