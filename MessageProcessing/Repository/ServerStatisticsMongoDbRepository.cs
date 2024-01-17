using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MessageProcessing.Repository
{
    public class ServerStatisticsMongoDbRepository : IRepository<ServerStatistics>
    {
        private readonly IMongoCollection<ServerStatistics> _collection;

        public ServerStatisticsMongoDbRepository(IOptions<MongoDbSettings> options)
        {
            var mongoDbSettings = options.Value;
            var _client = new MongoClient(mongoDbSettings.ConnectionString);
            var _database = _client.GetDatabase(mongoDbSettings.DatabaseName);
            _collection = _database.GetCollection<ServerStatistics>("ServerStatistics");
        }
        public async Task SaveAsync(ServerStatistics serverStatistics)
        {
           await _collection.InsertOneAsync(serverStatistics);
        }
    }
}
