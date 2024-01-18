using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace SignalRServer.AlertHubHandling
{
    public class AlertHubConnectionHandler : IHubConnection
    {
        private readonly HubConnection _hubConnection;

        public AlertHubConnectionHandler(IOptions<SignalRConfig> signalRConfig)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(signalRConfig.Value.SignalRUrl)
                .Build();
            ConfigureHubConnection();
        }

        private void ConfigureHubConnection()
        {
            _hubConnection.Closed += async (error) =>
            {
                Console.WriteLine($"Connection closed with error: {error}");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            _hubConnection.Reconnecting += (error) =>
            {
                Console.WriteLine($"Reconnecting due to error: {error}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                Console.WriteLine($"Reconnected with connection id: {connectionId}");
                return Task.CompletedTask;
            };

            _hubConnection.StartAsync();
        }

        public Task StartAsync() => _hubConnection.StartAsync();
        public Task StopAsync() => _hubConnection.StopAsync();
        public async Task SendAsync(string methodName, string message) => await _hubConnection.SendAsync(methodName, message);
    }
}