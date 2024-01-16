using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerStatisticsCollectionLibrary;
using ServerStatisticsCollectionService;



var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ServerStatisticsConfig>(
                    hostContext.Configuration.GetSection("ServerStatisticsConfig"));

                services.Configure<RabbitMQConfig>(
                    hostContext.Configuration.GetSection("MessagingSettings:RabbitMQConfig"));

                services.AddSingleton<IMessageQueuePublisher, RabbitMQPublisher>();
                services.AddSingleton<IServerStatisticsCollector, ServerStatisticsCollector>();
                services.AddSingleton<IServerStatisticsCollectorService, ServerStatisticsCollectorService>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var serverStatisticsCollectorService = services.GetRequiredService<IServerStatisticsCollectorService>();

serverStatisticsCollectorService.Run();
