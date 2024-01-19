using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRServer.AlertHubHandling
{
    public interface IHubConnection
    {
        Task StartAsync();
        Task StopAsync();
        Task SendAsync(string methodName, string message);
        void SubscribeHighUsageAlert(Action<string> callback);
        void SubscribeAnomalyAlert(Action<string> callback);
    }
}
