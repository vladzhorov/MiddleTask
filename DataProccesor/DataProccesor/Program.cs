using DataProccesor.Exstentions;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMongoDependensies(builder.Configuration);

builder.Services.AddRabbitMQDependensies(builder.Configuration);

var host = builder.Build();
await host.RunAsync();