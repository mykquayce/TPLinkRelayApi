using Microsoft.AspNetCore.Mvc.Testing;

namespace TPLinkRelayApi.Api.Tests;

public class WeatherForecastTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _httpClient = factory.CreateClient();

	[Theory, InlineData("weatherforecast")]
	public async Task Test1(string requestUri)
	{
		// Arrange
		using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);

		// Act
		var response = await _httpClient.GetAsync(requestUri, cts.Token);
		var content = await response.Content.ReadAsStringAsync(cts.Token);

		// Assert
		Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);
		Assert.NotEmpty(content);
		Assert.StartsWith("[", content);
	}
}
