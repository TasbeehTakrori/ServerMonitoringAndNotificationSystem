using ServerStatisticsCollectionLibrary.Models;
using System.Diagnostics;

namespace ServerStatisticsCollectionLibrary
{
    public class ServerStatisticsCollector : IServerStatisticsCollector
    {
        private readonly PerformanceCounter _committedBytesCounter = new("Memory", "Committed Bytes");
        private readonly PerformanceCounter _availableMemoryCounter = new("Memory", "Available Bytes");
        private readonly PerformanceCounter _cpuCounter = new("Processor", "% Processor Time", "_Total");

        public ServerStatistics CollectStatistics()
        {
            Thread.Sleep(1000);
            var memoryUsage = GetMemoryUsage();
            var availableMemory = GetAvailableMemory();
            var cpuUsage = GetCpuUsage();

            var statistics = new ServerStatistics
            {
                MemoryUsage = memoryUsage,
                AvailableMemory = availableMemory,
                CpuUsage = cpuUsage,
                Timestamp = DateTime.UtcNow
            };

            return statistics;
        }

        private double GetMemoryUsage()
        {
            var committedMemory = _committedBytesCounter.NextValue() / (1024 * 1024);
            return committedMemory - GetAvailableMemory();
        }

        private double GetAvailableMemory()
        {
            return _availableMemoryCounter.NextValue() / (1024 * 1024);
        }

        private double GetCpuUsage()
        {
            return _cpuCounter.NextValue() / 100;
        }
    }
}