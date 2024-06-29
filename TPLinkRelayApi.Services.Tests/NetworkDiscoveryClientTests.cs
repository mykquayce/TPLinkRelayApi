using Helpers.NetworkDiscovery;

namespace TPLinkRelayApi.Services.Tests;

public class NetworkDiscoveryClientTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IClient _sut = fixture.NetworkDiscoveryClient;

	[Fact]
	public async Task Test1()
	{
		// Act
		var leases = await _sut.GetAllLeasesAsync().ToArrayAsync();

		// Assert
		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);
		Assert.All(leases, l => Assert.NotEqual(default, l.Expiration));
		Assert.All(leases, l => Assert.NotEqual(default, l.PhysicalAddress));
		Assert.All(leases, l => Assert.NotEqual(default, l.IPAddress));
	}
}
