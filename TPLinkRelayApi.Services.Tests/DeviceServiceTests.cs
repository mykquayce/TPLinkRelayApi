using System.Net;
using System.Net.NetworkInformation;

namespace TPLinkRelayApi.Services.Tests;

[Collection(nameof(NonParallelCollectionDefinition))]
public class DeviceServiceTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IDeviceService _sut = fixture.DeviceService;

	[Theory]
	[InlineData("amp")]
	public async Task GetByAliasTests(string key)
	{
		// Act
		var device = await _sut.GetDeviceAsync(key);

		// Assert
		Assert.NotEqual(default, device);
		Assert.NotEqual(default, device.Alias);
	}

	[Theory]
	[InlineData("192.168.1.220")]
	public async Task GetByIPAddressTests(string ipString)
	{
		// Arrange
		var ip = IPAddress.Parse(ipString);

		// Act
		var device = await _sut.GetDeviceAsync(ip);

		// Assert
		Assert.NotEqual(default, device);
		Assert.NotEqual(default, device.Alias);
	}

	[Theory]
	[InlineData("003192e1a68b")]
	[InlineData("003192E1A68B")]
	[InlineData("00:31:92:e1:a6:8b")]
	[InlineData("00:31:92:E1:A6:8B")]
	[InlineData("00-31-92-e1-a6-8b")]
	[InlineData("00-31-92-E1-A6-8B")]
	public async Task GetByPMacAddressTests(string macString)
	{
		// Arrange
		var mac = PhysicalAddress.Parse(macString);

		// Act
		var device = await _sut.GetDeviceAsync(mac);

		// Assert
		Assert.NotEqual(default, device);
		Assert.NotEqual(default, device.Alias);
	}
}
