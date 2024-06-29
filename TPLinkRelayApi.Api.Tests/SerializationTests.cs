using Helpers.Json.Converters;
using System.Net;
using System.Text.Json;

namespace TPLinkRelayApi.Api.Tests;

public class SerializationTests
{
	private readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		Converters = { new JsonIPAddressConverter(), },
	};

	[Theory, InlineData(@"{""IPAddress"":""192.168.1.220""}")]
	public void IPAddressSerializationTests(string json)
	{
		var record = JsonSerializer.Deserialize<Record>(json, _jsonSerializerOptions);

		Assert.NotEqual(default, record);
		Assert.NotEqual(default, record.IPAddress);
	}

	[Theory, InlineData("192.168.1.220")]
	public void IPAddressDeserializationTests(string s)
	{
		var record = new Record(IPAddress: IPAddress.Parse(s));

		var json = JsonSerializer.Serialize(record, _jsonSerializerOptions);

		Assert.NotEmpty(json);
		Assert.NotEqual("{}", json);
		Assert.Contains(s, json);
	}

	private readonly record struct Record(IPAddress IPAddress);
}
