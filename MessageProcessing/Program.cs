using MessageProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQClientLibrary;

var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<RabbitMQConfig>(
                    hostContext.Configuration.GetSection("MessagingSettings:RabbitMQConfig"));

                services.AddSingleton<IMessageQueueConsumer<ServerStatistics>, RabbitMQConsumer<ServerStatistics>>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var consumer = services.GetRequiredService<IMessageQueueConsumer<ServerStatistics>>();

consumer.StartConsumingMessages("ServerStatistics.*");

Console.ReadKey();

