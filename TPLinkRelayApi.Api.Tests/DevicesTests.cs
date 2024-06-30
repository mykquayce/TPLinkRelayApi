using Microsoft.AspNetCore.Mvc.Testing;

namespace TPLinkRelayApi.Api.Tests;

public class DevicesTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _httpClient = factory.CreateClient();

	[Theory]
	[InlineData("amp")]
	[InlineData("003192e1a68b")]
	[InlineData("003192E1A68B")]
	[InlineData("00:31:92:e1:a6:8b")]
	[InlineData("00:31:92:E1:A6:8B")]
	[InlineData("00-31-92-e1-a6-8b")]
	[InlineData("00-31-92-E1-A6-8B")]
	public async Task InfoTests(string s)
	{
		var response = await _httpClient.GetAsync($"devices/{s}/info");
		var content = await response.Content.ReadAsStringAsync();

		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(content);
		Assert.NotEqual("{}", content);
	}

	[Theory]
	[InlineData("amp")]
	public async Task DataTests(string s)
	{
		var response = await _httpClient.GetAsync($"devices/{s}/data");
		var content = await response.Content.ReadAsStringAsync();

		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(content);
		Assert.NotEqual("{}", content);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "may switch devices off")]
	[Theory(Skip = "may switch devices off")]
	[InlineData("amp", false, true)]
	public async Task StateTests(string s, params bool[] states)
	{
		foreach (var state in states)
		{
			var response = await _httpClient.PutAsync($"devices/{s}/state/{state}", content: null);

			Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);

			await Task.Delay(millisecondsDelay: 2_000);
		}
	}
}
