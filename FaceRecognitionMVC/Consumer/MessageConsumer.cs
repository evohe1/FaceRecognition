using FaceRecognitionMVC.Models;
using MassTransit;

namespace FaceRecognitionMVC.Consumer
{
    public class MessageConsumer :
        IConsumer<Message>
    {
       

        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Received Text: {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
