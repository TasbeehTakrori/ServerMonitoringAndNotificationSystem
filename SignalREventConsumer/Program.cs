using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalREventConsuming;
using SignalRServer.AlertHubHandling;


var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                           .AddConsole();
                });

                services.Configure<SignalRServer.AlertHubHandling.SignalRConfig>(
                                    hostContext.Configuration.GetSection("SignalRConfig"));

                services.AddSingleton<IHubConnection, AlertHubConnectionHandler>();
                services.AddSingleton<ISignalREventConsumer, SignalREventConsumer>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;

var signalREventConsumer = services.GetRequiredService<ISignalREventConsumer>();
await signalREventConsumer.StartAsync();

Console.ReadKey();
