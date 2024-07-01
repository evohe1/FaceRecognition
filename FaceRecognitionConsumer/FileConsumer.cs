using FaceRecognition.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FaceRecognitionConsumer
{
    public class FileConsumer : IConsumer<FileMessage>
    {
        
        private readonly ILogger<FileConsumer> _logger;

        private readonly string _cascadeFilePath;

        public FileConsumer(ILogger<FileConsumer> logger)
        {
            _logger = logger;
            _cascadeFilePath = Path.Combine(AppContext.BaseDirectory, "Cascades", "haarcascade_frontalface_default.xml");
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

                if (IsImageFile(filePath))
                {
                    using var image = new Mat(filePath, ImreadModes.Color);
                    var face = ExtractFace(image);

                    if (face != null)
                    {
                        var facePath = Path.Combine("ReceivedFiles", $"face_{fileMessage.FileName}");
                        face.SaveImage(facePath);
                        _logger.LogInformation("Face detected and saved: {FilePath}", facePath);
                    }
                    else
                    {
                        _logger.LogWarning("No face detected in the image: {FileName}", fileMessage.FileName);
                    }
                }
                else
                {
                    _logger.LogWarning("Received file is not a valid image: {FileName}", fileMessage.FileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the file: {FileName}", fileMessage.FileName);
            }
        }

        private bool IsImageFile(string filePath)
        {
            try
            {
                using var img = System.Drawing.Image.FromFile(filePath);
                return true;
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        private Mat ExtractFace(Mat image)
        {
            var faceCascade = new CascadeClassifier(_cascadeFilePath);
            var grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            var faces = faceCascade.DetectMultiScale(
                grayImage,
                scaleFactor: 1.1,
                minNeighbors: 5,
                minSize: new OpenCvSharp.Size(30, 30)
            );

            if (faces.Length == 0)
                return null;

            var faceRect = faces[0];
            var face = new Mat(image, faceRect);
            return face;
        }
    }
}
