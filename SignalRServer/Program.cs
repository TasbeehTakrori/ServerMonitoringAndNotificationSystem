using SignalRServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<AlertHub>("/AlertHub");

app.Run();