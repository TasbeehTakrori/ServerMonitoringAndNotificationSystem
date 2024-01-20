using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace SignalRServer.AlertHubHandling
{
    public class AlertHubConnectionHandler : IHubConnection
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<AlertHubConnectionHandler> _logger;
        private readonly Random _random = new Random();

        public AlertHubConnectionHandler(
            IOptions<SignalRConfig> signalRConfig,
            ILogger<AlertHubConnectionHandler> logger)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(signalRConfig.Value.SignalRUrl)
                .Build();
            _logger = logger;
            ConfigureHubConnection();
            Task.Run(async () => await _hubConnection.StartAsync()).Wait();
        }

        private void ConfigureHubConnection()
        {
            _hubConnection.Closed += async (error) =>
            {
                _logger.LogError($"Connection closed with error: {error}");
                _logger.LogInformation("Attempting to reconnect...");
                await Task.Delay(_random.Next(0, 5) * 1000);
                await StartAsync();
            };

            _hubConnection.Reconnecting += (error) =>
            {
                _logger.LogWarning($"Reconnecting due to error: {error}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                _logger.LogInformation($"Reconnected with connection id: {connectionId}");
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting connection: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            try
            {
                await _hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error stopping connection: {ex.Message}");
            }
        }

        public async Task SendAsync(string methodName, string message)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync(methodName, message);
            }
            else
            {
                _logger.LogWarning("Cannot send message, connection is not in a connected state.");
            }
        }

        public void SubscribeHighUsageAlert(Action<string> callback)
        {
            _hubConnection.On<string>("ReceiveHighUsageAlert", message =>
            {
                callback.Invoke(message);
            });
        }

        public void SubscribeAnomalyAlert(Action<string> callback)
        {
            _hubConnection.On<string>("ReceiveAnomalyAlert", message =>
            {
                callback.Invoke(message);
            });
        }
    }
}
