using Helpers.Json.Converters;
using Helpers.NetworkDiscovery;
using Helpers.NetworkDiscovery.Concrete;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using TPLinkRelayApi.Services;
using TPLinkRelayApi.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<OtlpExporterOptions>(
	o => o.Headers = "x-otlp-api-key=c0b35c056105346aa1a89f3ce0bb8bf9c0c324182e40d4ec1a4e51a2a367a536");

builder.Logging.AddOpenTelemetry(x =>
{
	x.IncludeScopes = true;
	x.IncludeFormattedMessage = true;
});

builder.Services.AddOpenTelemetry()
	.WithMetrics(x =>
	{
		x.AddRuntimeInstrumentation()
			.AddMeter(
				"Microsoft.AspNetCore.Hosting",
				"Microsoft.AspNetCore.Server.Kestrel",
				"System.Net.Http",
				"TPLinkRelayApi.Api");
	})
	.WithTracing(x =>
	{
		if (builder.Environment.IsDevelopment())
		{
			x.SetSampler<AlwaysOnSampler>();
		};

		x.AddAspNetCoreInstrumentation()
			.AddGrpcClientInstrumentation()
			.AddHttpClientInstrumentation();
	});

builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());

builder.Services.AddHealthChecks()
	.AddCheck("self", () => HealthCheckResult.Healthy(), ["live",]);

builder.Services.AddMetrics();

builder.Services.ConfigureHttpClientDefaults(http =>
{
	http.AddStandardResilienceHandler();
});

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

builder.Services.AddMetrics();

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapHealthChecks("/alive", new HealthCheckOptions
{
	Predicate = r => r.Tags.Contains("live"),
});

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
