namespace SignalRServer
{
    public interface IAlertHub
    {
        Task ReceiveAnomalyAlert(string message);
        Task ReceiveHighUsageAlert(string message);
    }
}
