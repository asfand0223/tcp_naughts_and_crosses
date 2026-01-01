using Microsoft.Extensions.Hosting;
using TcpServer.Extensions.HostExtensions;
using TcpServer.Extensions.ServiceCollectionExtensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureServices();
builder.Services.ConfigureLogging();

var app = builder.Build();

app.UseGlobalExceptionHandling();

app.Run();
