using Microsoft.AspNetCore.Mvc.Testing;

namespace TPLinkRelayApi.Api.Tests;

public class DevicesTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _httpClient = factory.CreateClient();

	[Theory]
	[InlineData("/devices/amp/info")]
	[InlineData("/devices/003192e1a68b/info")]
	[InlineData("/devices/003192E1A68B/info")]
	[InlineData("/devices/00:31:92:e1:a6:8b/info")]
	[InlineData("/devices/00:31:92:E1:A6:8B/info")]
	[InlineData("/devices/00-31-92-e1-a6-8b/info")]
	[InlineData("/devices/00-31-92-E1-A6-8B/info")]
	public async Task InfoTests(string requestUri)
	{
		var response = await _httpClient.GetAsync(requestUri);
		var content = await response.Content.ReadAsStringAsync();

		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(content);
		Assert.NotEqual("{}", content);
	}
}
