namespace TPLinkRelayApi.Services.Tests;

public class NetworkDiscoveryServiceTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly INetworkDiscoveryService _sut = fixture.NetworkDiscoveryService;

	[Fact]
	public async Task GetAllTPLinkDevicesTests()
	{
		// Act
		var devices = await _sut.GetAllTPLinkDevicesAsync().ToArrayAsync();

		// Assert
		Assert.NotEmpty(devices);
		Assert.DoesNotContain(default, devices);
		Assert.All(devices, d => Assert.Equal(new byte[3] { 0x00, 0x31, 0x92, }, d.PhysicalAddress.GetAddressBytes()[..3]));
	}
}
