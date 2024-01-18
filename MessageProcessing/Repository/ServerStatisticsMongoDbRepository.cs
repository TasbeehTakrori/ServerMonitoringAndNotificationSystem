using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MessageProcessing.Repository
{
    public class ServerStatisticsMongoDbRepository : IRepository
    {
        private readonly IMongoCollection<ServerStatistics> _collection;
        private readonly ILogger<ServerStatisticsMongoDbRepository> _logger;

        public ServerStatisticsMongoDbRepository(
            IOptions<MongoDbSettings> options,
            ILogger<ServerStatisticsMongoDbRepository> logger)
        {
            var mongoDbSettings = options.Value;
            var _client = new MongoClient(mongoDbSettings.ConnectionString);
            var _database = _client.GetDatabase(mongoDbSettings.DatabaseName);
            _collection = _database.GetCollection<ServerStatistics>("ServerStatistics");
            _logger = logger;
        }

        public async Task SaveAsync(ServerStatistics serverStatistics)
        {
            try
            {
                await _collection.InsertOneAsync(serverStatistics);
                _logger.LogInformation($"Saved server statistics for {serverStatistics.ServerIdentifier}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving server statistics: {ex.Message}");
                throw;
            }
        }

        public async Task<ServerStatistics> GetLastRecordForServer(string serverIdentifier)
        {
            try
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

                _logger.LogInformation($"Retrieved last record for {serverIdentifier}.");
                return lastRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving last record: {ex.Message}");
                throw;
            }
        }
    }
}