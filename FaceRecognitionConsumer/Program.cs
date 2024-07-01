// See https://aka.ms/new-console-template for more information

using FaceRecognitionConsumer;
using MassTransit;
using MassTransit.AspNetCoreIntegration;
using MassTransit.Transports.Fabric;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");
var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
var rabbitMqOptions = configuration.GetSection("RabbitMQ").Get<RabbitMqOptions>();
var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<FileConsumer>();

                        x.UsingRabbitMq((ctx, cfg) =>
                        {
                            cfg.Host(rabbitMqOptions.Host, h =>
                            {
                                h.Username(rabbitMqOptions.Username);
                                h.Password(rabbitMqOptions.Password);
                            });

                            cfg.ReceiveEndpoint("file-upload-queue", e =>
                            {
                                
                                e.ConfigureConsumer<FileConsumer>(ctx);
                            });
                            cfg.ConfigureEndpoints(ctx);
                        });
                    });

                   

                    services.AddLogging(configure => configure.AddConsole());
                })
                .Build();

await host.RunAsync();
        



