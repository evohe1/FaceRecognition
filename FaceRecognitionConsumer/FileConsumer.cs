using FaceRecognition.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionConsumer
{
    public class FileConsumer : IConsumer<FileMessage>
    {
        
        private readonly ILogger<FileConsumer> _logger;

        public FileConsumer(ILogger<FileConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FileMessage> context)
        {
            var fileMessage = context.Message;
            var filePath = Path.Combine("ReceivedFiles", fileMessage.FileName);

            try
            {
                Directory.CreateDirectory("ReceivedFiles");
                await File.WriteAllBytesAsync(filePath, fileMessage.FileContent);
                _logger.LogInformation("File received and saved: {FileName}, Size: {Size} bytes",
                    fileMessage.FileName, fileMessage.FileContent.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving the file: {FileName}", fileMessage.FileName);
            }
        }
    }
}
