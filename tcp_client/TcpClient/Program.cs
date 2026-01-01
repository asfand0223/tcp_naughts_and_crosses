using Microsoft.Extensions.Hosting;
using TcpClient.Extensions.HostExtensions;
using TcpClient.Extensions.ServiceCollectionExtensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureServices();
builder.Services.ConfigureLogging();

var app = builder.Build();

app.UseGlobalExceptionHandling();

app.Run();
