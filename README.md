# Building a Server Monitoring and Notification System

This project involves the development of a server monitoring and notification system with multiple components, including a server statistics collection service, message processing and anomaly detection service, and a SignalR event consumer service.

## Task 1: Server Statistics Collection Service

### Objective

Develop a C# process that collects and publishes server statistics, specifically memory usage, available memory, and CPU usage.

### Requirements

- Collect server statistics (memory usage, available memory, and CPU usage) at regular intervals, specified by a configurable period `SamplingIntervalSeconds`, using the `System.Diagnostics` namespace.
- Encapsulate the collected statistics (memory usage, available memory, CPU usage, and timestamp) into a statistics object.
- Publish the statistics object to a message queue under the topic `ServerStatistics.<ServerIdentifier>`, where `<ServerIdentifier>` is a configurable option.
- Implement an abstraction for message queuing to ensure the code is not tightly coupled to RabbitMQ.

![image](https://github.com/TasbehTakrore/ServerMonitoringAndNotificationSystem/assets/71009816/7f757124-ac1d-4d3c-83cd-2fce8db96829)

#### Code Snippet: Server Statistics Data Type (Class Definition)

```csharp
public class ServerStatistics
{
    public double MemoryUsage { get; set; } // in MB
    public double AvailableMemory { get; set; } // in MB
    public double CpuUsage { get; set; }
    public DateTime Timestamp { get; set; }
}
```

Code Snippet: Server Statistics Configuration (appsettings.json)
```json
{
    "ServerStatisticsConfig": {
        "SamplingIntervalSeconds": 60,
        "ServerIdentifier": "linux1"
    }
}
```

## Task 2: Message Processing and Anomaly Detection Service
### Objective
Develop a C# process that receives server statistics from the message queue, persists the data to a MongoDB instance, and sends alerts if anomalies or high usage is detected.

### Requirements
Receive messages from the message queue with the topic ServerStatistics.* and deserialize them into objects containing memory usage, available memory, CPU usage, server's identifier, and timestamp.
Persist the deserialized data to a MongoDB instance.
Implement anomaly detection logic based on configurable threshold percentages specified in appsettings.json.
Send an "Anomaly Alert" via SignalR if a sudden increase (anomaly) in memory usage or CPU usage is detected.
Send a "High Usage Alert" via SignalR if the calculated memory usage percentage or CPU usage exceeds the specified "Usage Threshold".
Implement abstractions for MongoDB and message queuing to ensure the code is not tightly coupled to specific implementations.

![image](https://github.com/TasbehTakrore/ServerMonitoringAndNotificationSystem/assets/71009816/7aab141d-6536-4ce2-84f0-1ee9bf3ae1e3)


Equations for Alert Calculation
For Anomaly Alert:
- Memory Usage Anomaly Alert:``` if (CurrentMemoryUsage > (PreviousMemoryUsage * (1 + MemoryUsageAnomalyThresholdPercentage)))```
- CPU Usage Anomaly Alert: ```if (CurrentCpuUsage > (PreviousCpuUsage * (1 + CpuUsageAnomalyThresholdPercentage)))```
For High Usage Alert:
- Memory High Usage Alert: ```if ((CurrentMemoryUsage / (CurrentMemoryUsage + CurrentAvailableMemory)) > MemoryUsageThresholdPercentage)```
- CPU High Usage Alert: ```if (CurrentCpuUsage > CpuUsageThresholdPercentage)```

Code Snippet: Anomaly Detection Service Configuration (appsettings.json)
```json
{
    "AnomalyDetectionConfig": {
        "MemoryUsageAnomalyThresholdPercentage": 0.4,
        "CpuUsageAnomalyThresholdPercentage": 0.5,
        "MemoryUsageThresholdPercentage": 0.8,
        "CpuUsageThresholdPercentage": 0.9
    },
    "SignalRConfig": {
        "SignalRUrl": "<YourSignalRHubURL>"
    }
}
```

## Task 3: SignalR Event Consumer Service
### Objective
Develop a C# process that connects to a SignalR hub and prints received events to the console.

### Requirements
Establish a connection to a SignalR hub.
Subscribe to events and print them to the console as they are received.

![image](https://github.com/TasbehTakrore/ServerMonitoringAndNotificationSystem/assets/71009816/5ef1bfd4-12f8-485d-9398-c9fed3c391cb)

Code Snippet: SignalR Event Consumer Service Configuration (appsettings.json)
```json
{
    "SignalRConfig": {
        "SignalRUrl": "<YourSignalRHubURL>"
    }
}
```

## Task 4: Create a Reusable RabbitMQ Client Library
### Objective
Develop a reusable class library for interacting with RabbitMQ, thereby abstracting the RabbitMQ client and making it easier to switch to a different message queuing system in the future.

### Requirements
Implement a class for publishing messages to a RabbitMQ exchange.
Implement a class for consuming messages from a RabbitMQ queue.
Both classes should utilize an interface, making the library extensible and not tightly coupled to RabbitMQ.
