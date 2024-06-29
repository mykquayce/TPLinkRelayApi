using Helpers.Json.Converters;
using Helpers.NetworkDiscovery;
using Helpers.NetworkDiscovery.Concrete;
using TPLinkRelayApi.Services;
using TPLinkRelayApi.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddMemoryCache()
	.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
	.AddCachingHandler(b => b.Expiration = TimeSpan.FromHours(1))
	.AddHttpClient<IClient, Client>(name: "NetworkDiscoveryClient", c => c.BaseAddress = new Uri("https://networkdiscovery/"))
		.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
		.AddHttpMessageHandler<CachingHandler>()
		.Services
	.AddTransient<INetworkDiscoveryService, NetworkDiscoveryService>()
	.AddTPLink()
	.AddTransient<IDeviceService, DeviceService>();


builder.Services.AddControllers()
	.AddJsonOptions(c =>
	{
		c.JsonSerializerOptions.Converters.Add(new JsonIPAddressConverter());
		c.JsonSerializerOptions.Converters.Add(new JsonPhysicalAddressConverter());
	});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
