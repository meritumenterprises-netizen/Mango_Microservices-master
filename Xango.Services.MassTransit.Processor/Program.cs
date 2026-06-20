using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xango.Service.AuthenticationAPI.Client;
using Xango.Service.OrderAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.MassTransit.Processor;
using Xango.Services.Server.Utility;

var configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true)
	.AddEnvironmentVariables()
	.Build();

await Host.CreateDefaultBuilder(args)
	.ConfigureLogging(logging =>
	{
		logging.ClearProviders();
		logging.AddConsole();
		logging.SetMinimumLevel(LogLevel.Information);
	})
	.ConfigureServices(services =>
	{
		services.AddSingleton<IConfiguration>(configuration);
		services.AddHttpClient();
		services.AddHttpContextAccessor();
		services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
		services.AddScoped<ITokenProvider, TokenProvider>();
		services.AddTransient<IAuthenticationHttpClient, AuthenticationHttpClient>();
		services.AddTransient<IOrderHttpClient, OrderHttpClient>();
		services.AddHostedService<OrderQueuePollingService>();
	})
	.Build()
	.RunAsync();
