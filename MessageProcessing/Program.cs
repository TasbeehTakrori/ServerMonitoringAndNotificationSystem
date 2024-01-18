using MessageProcessing;
using MessageProcessing.MessageHandling;
using MessageProcessing.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQClientLibrary;
using SignalRServer.AlertHubHandling;

var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<MongoDbSettings>(
                    hostContext.Configuration.GetSection("DatabaseSettings"));
                services.Configure<RabbitMQConfig>(
                    hostContext.Configuration.GetSection("MessagingSettings:RabbitMQConfig"));
                services.Configure<SignalRConfig>(
                    hostContext.Configuration.GetSection("SignalRConfig"));

                services.AddSingleton<IHubConnection, AlertHubConnectionHandler>();
                services.AddSingleton<IMessageQueueConsumer<ServerStatistics>, RabbitMQConsumer<ServerStatistics>>();
                services.AddSingleton<IRepository<ServerStatistics>, ServerStatisticsMongoDbRepository>();
                services.AddSingleton<IMessageHandler<ServerStatistics>, MessageHandler<ServerStatistics>>();
                services.AddSingleton<IMessageProcessor, MessageProcessor>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var processor = services.GetRequiredService<IMessageProcessor>();

processor.Run();

Console.ReadKey();
