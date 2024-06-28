using FaceRecognitionMVC.Consumer;
using FaceRecognitionMVC.Options;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var rabbitMqOptions =builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>();
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqOptions.Host, h =>
        {
            h.Username(rabbitMqOptions.Username);
            h.Password(rabbitMqOptions.Password);
        });

       
        cfg.ReceiveEndpoint("queue:faceRecognition", e =>
        {
            e.ConfigureConsumer<MessageConsumer>(context);
        });
    });

    x.AddConsumer<MessageConsumer>();
});

builder.Services.AddMassTransitHostedService();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
