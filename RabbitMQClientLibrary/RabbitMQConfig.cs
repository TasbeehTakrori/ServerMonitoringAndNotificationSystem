﻿namespace RabbitMQClientLibrary
{
    public class RabbitMQConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeType { get; set; }
        public string ExchangeName { get; set; }
    }
}
