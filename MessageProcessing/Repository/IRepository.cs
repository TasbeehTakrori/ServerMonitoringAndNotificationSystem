namespace MessageProcessing.Repository
{
    public interface IRepository
    {
        Task SaveAsync(ServerStatistics data);
        Task<ServerStatistics> GetLastRecordForServer(string serverIdentifier);
    }
}