using MessageProcessing;
using MessageProcessing.AnomalyDetection;
using MessageProcessing.MessageHandling;
using MessageProcessing.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                services.Configure<AnomalyDetectionConfig>(
                 hostContext.Configuration.GetSection("AnomalyDetectionConfig"));

                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                           .AddConsole();
                });

                services.AddSingleton<IHubConnection, AlertHubConnectionHandler>();
                services.AddSingleton<IAnomalyDetector, AnomalyDetector>();
                services.AddSingleton<IAnomalyHandler, AnomalyHandler>();
                services.AddSingleton<IMessageQueueConsumer<ServerStatistics>, RabbitMQConsumer<ServerStatistics>>();
                services.AddSingleton<IRepository, ServerStatisticsMongoDbRepository>();
                services.AddSingleton<IMessageHandler, MessageHandler>();
                services.AddSingleton<IMessageProcessor, MessageProcessor>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;


var processor = services.GetRequiredService<IMessageProcessor>();

processor.Run();

Console.ReadKey();
