using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQClientLibrary;
using ServerStatisticsCollectionLibrary;
using ServerStatisticsCollectionLibrary.Models;
using ServerStatisticsCollectionService;
using System.Threading.Tasks;


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

                services.AddSingleton<IMessageQueuePublisher<ServerStatistics>, RabbitMQPublisher<ServerStatistics>>();
                services.AddSingleton<IServerStatisticsCollector, ServerStatisticsCollector>();
                services.AddSingleton<IServerStatisticsCollectorService, ServerStatisticsCollectorService>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var serverStatisticsCollectorService = services.GetRequiredService<IServerStatisticsCollectorService>();

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var task = Task.Run(() => serverStatisticsCollectorService.RunAsync(cancellationToken));

Console.WriteLine("Press any key to stop...");
Console.ReadKey();

cancellationTokenSource.Cancel();

await task;