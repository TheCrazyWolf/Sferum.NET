using SferumSharp.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<VkFactory>();

var host = builder.Build();
host.Run();