// See https://aka.ms/new-console-template for more information

using FaceRecognitionConsumer;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");
var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
var rabbitMqOptions = configuration.GetSection("RabbitMQ").Get<RabbitMqOptions>();
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host(rabbitMqOptions.Host, h =>
    {
        h.Username(rabbitMqOptions.Username);
        h.Password(rabbitMqOptions.Password);
    });

    cfg.ReceiveEndpoint("queue:faceRecognition", e =>
    {
        e.Consumer<MessageConsumer>();
    });
});

// Bus başlat
await busControl.StartAsync();
try
{
    Console.WriteLine("Press enter to exit");
    await Task.Run(() => Console.ReadLine());

    // Mesaj gönder
    await busControl.Publish(new Message { Text = "Hello, World!" });
}
finally
{
    await busControl.StopAsync();
}