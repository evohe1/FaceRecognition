using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionConsumer
{
    public class MessageConsumer:IConsumer<Message>
    {
        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Received Text: {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
