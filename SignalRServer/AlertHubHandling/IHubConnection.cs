namespace SignalRServer.AlertHubHandling
{
    public interface IHubConnection
    {
        Task StartAsync();
        Task StopAsync();
        Task SendAsync(string methodName, string message);
    }
}
