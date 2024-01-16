namespace ServerStatisticsCollectionService
{
    internal interface IServerStatisticsCollectorService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}