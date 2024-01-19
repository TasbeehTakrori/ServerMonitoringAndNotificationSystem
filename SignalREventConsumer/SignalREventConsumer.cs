using Microsoft.Extensions.Logging;
using SignalRServer.AlertHubHandling;

namespace SignalREventConsuming
{
    internal class SignalREventConsumer : ISignalREventConsumer
    {
        private readonly IHubConnection _hubConnection;
        private readonly ILogger<SignalREventConsumer> _logger;
        public SignalREventConsumer(
            IHubConnection hubConnection,
            ILogger<SignalREventConsumer> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
        }

        public void Run()
        {
            try
            {
                _hubConnection.SubscribeHighUsageAlert(message =>
                {
                    _logger.LogInformation($"Received HighUsage Alert: {message}");
                });

                _hubConnection.SubscribeAnomalyAlert(message =>
                {
                    _logger.LogInformation($"Received Anomaly Alert: {message}");
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
