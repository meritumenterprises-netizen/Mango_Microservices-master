using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Threading;
using Xango.Service.AuthenticationAPI.Client;
using Xango.Service.OrderAPI.Client;
using Xango.Services.Queue.Processor;
using Xango.Services.RabbitMQ;
using Xango.Services.Server.Utility;

public class Program
{
	public static void Main(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var builder = Host.CreateDefaultBuilder(args)
			.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
			logging.SetMinimumLevel(LogLevel.Information);
		})
		.ConfigureServices(services =>
		{
			services.AddSingleton<OrdersPendingProcessor>();
			services.AddSingleton<OrdersCancelledProcessor>();
			services.AddSingleton<OrdersReadyForPickupProcessor>();
			services.AddSingleton<OrdersApprovedProcessor>();
			services.AddSingleton<RabbitMqReader>();
			services.AddHttpClient();

			services.AddSingleton<IConfiguration>(configuration);
			services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
			services.AddTransient<IAuthenticationHttpClient, AuthenticationHttpClient>();
			services.AddTransient<IOrderHttpClient, OrderHttpClient>();
			services.AddTransient<ITokenProvider, TokenProvider>();
			services.AddSingleton<IConnectionFactory>((serviceProvider) =>
			{
				return new ConnectionFactory
				{
					HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
					UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER"),
					Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"),
					AutomaticRecoveryEnabled = true,
					NetworkRecoveryInterval = TimeSpan.FromSeconds(15),
					TopologyRecoveryEnabled = true,
					RequestedHeartbeat = TimeSpan.FromSeconds(30)
				};
			});
			services.AddSingleton<IConnection>(sp =>
			{
				var factory = sp.GetRequiredService<IConnectionFactory>();
				return factory.CreateConnection();
			});
			services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
			services.AddHttpContextAccessor();
		});

		var host = builder.Build();
		
		var serviceProvider = host.Services;

		try
		{
			Thread.Sleep(30000); // Wait for dependent services to be ready, this is a simple approach, consider using a more robust solution in production
			Console.WriteLine("[Xango.Services.Queue.Processor] Starting");
			var maxRuntimeMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_RUNTIME_MINUTES"));
			if (maxRuntimeMinutes > 0)
			{
				Console.WriteLine($"[Xango.Services.QueueProcessor] the process will run for max. {maxRuntimeMinutes} minutes");
			}
			else
			{
				Console.WriteLine("[Xango.Services.QueueProcessor] the process will run indefinitely");
			}
			var mqReader = new RabbitMqReader(serviceProvider);
			mqReader.StartAsync();
			host.Run();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[Xango.Services.QueueProcessor] Exception: {ex.Message}");
		}
		Console.WriteLine("[Xango.Services.Queue.Processor] Exiting");
	}
}