using Microsoft.AspNetCore.SignalR;

namespace SignalRServer
{
    public class AlertHub : Hub<IAlertHub>
    {
        private readonly ILogger<AlertHub> _logger;

        public AlertHub(ILogger<AlertHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendAnomalyAlertMessage(string message)
        {
            _logger.LogInformation($"Sending anomaly alert message: {message}");

            try
            {
                await Clients.All.ReceiveAnomalyAlert(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending anomaly alert message: {ex.Message}");
            }
        }

        public async Task SendHighUsageAlertMessage(string message)
         {
            _logger.LogInformation($"Sending high usage alert message: {message}");

            try
            {
                await Clients.All.ReceiveHighUsageAlert(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending high usage alert message: {ex.Message}");
            }
        }
    }
}
