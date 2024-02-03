using MessageProcessing.AnomalyDetection.Enum;
using Microsoft.Extensions.Logging;
using SignalRServer.AlertHubHandling;

namespace MessageProcessing.AnomalyDetection
{
    public class AnomalyHandler : IAnomalyHandler
    {
        private readonly IHubConnection _hubConnection;
        private readonly ILogger<AnomalyHandler> _logger;
        public AnomalyHandler(
            IHubConnection hubConnection,
            ILogger<AnomalyHandler> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
        }

        public async Task HandleAnomalies(List<AnomalyType> anomalies, string serverIdentifier, DateTime timestamp)
        {
            foreach (var anomaly in anomalies)
            {
                if (IsAnomaly(anomaly))
                    await SendAlert(AnomalyMessageType.SendAnomalyAlertMessage, anomaly, serverIdentifier, timestamp);
                else if (IsHighUsageAnomaly(anomaly))
                    await SendAlert(AnomalyMessageType.SendHighUsageAlertMessage, anomaly, serverIdentifier, timestamp);
            }
        }

        private bool IsAnomaly(AnomalyType anomaly)
        {
            return anomaly == AnomalyType.CPUUsageAnomaly || anomaly == AnomalyType.MemoryUsageAnomaly;
        }

        private bool IsHighUsageAnomaly(AnomalyType anomaly)
        {
            return anomaly == AnomalyType.HighCPUUsage || anomaly == AnomalyType.HighMemoryUsage;
        }

        private async Task SendAlert(AnomalyMessageType messageType, AnomalyType anomalyType, string serverIdentifier, DateTime timestamp)
        {
            string methodName = System.Enum.GetName(typeof(AnomalyMessageType), messageType)!;
            await _hubConnection.SendAsync(methodName, $"{anomalyType} Detected! Server: {serverIdentifier}, Timestamp: {timestamp}");
            _logger.LogInformation($"{anomalyType} alert sent to SignalR! Server: {serverIdentifier}, Timestamp: {timestamp}");
        }
    }
}
