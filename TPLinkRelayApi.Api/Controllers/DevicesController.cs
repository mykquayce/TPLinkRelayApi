using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.NetworkInformation;
using TPLinkRelayApi.Models;
using TPLinkRelayApi.Services;

namespace TPLinkRelayApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DevicesController(IDeviceService deviceService, Helpers.TPLink.IService tpLinkService) : ControllerBase
{
	[HttpGet("{device:required}/info")]
	public async Task<IActionResult> GetDeviceInfo([FromRoute(Name = "device")] string s)
	{
		Device device;
		try { device = await FindDeviceAsync(s); }
		catch (KeyNotFoundException)
		{
			return base.BadRequest(error: new { message = "device not found", });
		}

		var info = await tpLinkService.GetSystemInfoAsync(device.IPAddress);

		return base.Ok(new
		{
			info.alias,
			info.model,
			device.IPAddress,
			device.PhysicalAddress,
			On = info.relay_state > 0,
		});
	}

	[HttpGet("{device:required}/data")]
	public async Task<IActionResult> GetDeviceData([FromRoute(Name = "device")] string s)
	{
		Device device;
		try { device = await FindDeviceAsync(s); }
		catch (KeyNotFoundException)
		{
			return base.BadRequest(error: new { message = "device not found", });
		}

		var (amps, volts, watts) = await tpLinkService.GetRealtimeDataAsync(device.IPAddress);
		return base.Ok(new { amps, volts, watts, });
	}

	[HttpPut("{device:required}/state/{state:bool}")]
	public async Task<IActionResult> SetDeviceState([FromRoute(Name = "device")] string s, bool state)
	{
		Device device;
		try { device = await FindDeviceAsync(s); }
		catch (KeyNotFoundException)
		{
			return base.BadRequest(error: new { message = "device not found", });
		}

		await tpLinkService.SetStateAsync(device.IPAddress, state);
		return base.Ok();
	}

	private Task<Device> FindDeviceAsync(string s, CancellationToken cancellationToken = default)
	{
		if (IPAddress.TryParse(s, out var ipAddress))
		{
			return deviceService.GetDeviceAsync(ipAddress, cancellationToken);
		}

		if (PhysicalAddress.TryParse(s, out var physicalAddress))
		{
			return deviceService.GetDeviceAsync(physicalAddress, cancellationToken);
		}

		return deviceService.GetDeviceAsync(s, cancellationToken);
	}
}
