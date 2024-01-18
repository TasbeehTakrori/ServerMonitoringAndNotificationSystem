using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MessageProcessing.Repository
{
    public class ServerStatisticsMongoDbRepository : IRepository
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
        public async Task<ServerStatistics> GetLastRecordForServer(string serverIdentifier)
        {
            var filter = Builders<ServerStatistics>.Filter.Eq(s => s.ServerIdentifier, serverIdentifier);
            var sort = Builders<ServerStatistics>.Sort.Descending(s => s.Timestamp);
            var projectionDefinition = Builders<ServerStatistics>.Projection.Exclude("_id");
            
            var lastRecord = await _collection
                .Find(filter)
                .Sort(sort)
                .Limit(1)
                .Project<ServerStatistics>(projectionDefinition)
                .FirstOrDefaultAsync();
            return lastRecord;
        }
    }
}
