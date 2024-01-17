using MessageProcessing;
using MessageProcessing.Repository;
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
                services.Configure<MongoDbSettings>(
                    hostContext.Configuration.GetSection("DatabaseSettings"));
                services.Configure<RabbitMQConfig>(
                    hostContext.Configuration.GetSection("MessagingSettings:RabbitMQConfig"));

                services.AddSingleton<IMessageQueueConsumer<ServerStatistics>, RabbitMQConsumer<ServerStatistics>>();
                services.AddSingleton<IRepository, ServerStatisticsMongoDbRepository>();
            })
            .Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;
var consumer = services.GetRequiredService<IMessageQueueConsumer<ServerStatistics>>();

IRepository db = services.GetRequiredService<IRepository>();

ServerStatistics serverStats = new ServerStatistics
{
    ServerIdentifier = "Server001",
    MemoryUsage = 512.5,
    AvailableMemory = 1024.8,
    CpuUsage = 25.3,
    Timestamp = DateTime.Now
};

db.Add(serverStats);

consumer.StartConsumingMessages("ServerStatistics.*");

Console.ReadKey();

//TODO => handling service
//TODO => MessageProcessing class