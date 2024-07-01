using FaceRecognition.Shared;
using FaceRecognitionMVC.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FaceRecognitionMVC.Controllers
{
    public class FileController : Controller
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IConfiguration _configuration;
        
        public FileController(ISendEndpointProvider sendEndpointProvider, IConfiguration configuration)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileMessage = new FileMessage
            {
                FileName = file.FileName,
                FileContent = memoryStream.ToArray()
            };
            var queueName = _configuration["RabbitMQ:QueueName"];
            var queueHost = _configuration["RabbitMQ:Host"];
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("rabbitmq://fish.rmq.cloudamqp.com/dvakdhvc/file-upload-queue"));
            await sendEndpoint.Send(fileMessage);

            return RedirectToAction("Upload");
        }
    }
}
