using DetectifyLambdaServices.Services;
using DetectifyLambdaServices.Services.Interfaces;
using DetectifyLambdaServices.Utils;
using SixLabors.ImageSharp;

namespace CelebrityRecognition
{
    public class Function
    {
        private readonly S3Service _s3Service;
        private readonly IRekognitionService _rekognitionService;

        public Function(S3Service s3Service, IRekognitionService rekognitionService)
            => (_s3Service, _rekognitionService) = (s3Service, rekognitionService);

        public async Task<string> CelebrityRecognition(float confidence, string boundingBoxColor, string labelColor)
        {
            var objectStream = await _s3Service.GetObjectStreamAsync();
            var objectType = await _s3Service.GetObjectType();

            var detectedFaces = await _rekognitionService.RecognizeCelebritiesSingleImage(objectStream);

            var image = SixLabors.ImageSharp.Image.Load(objectStream);
            BoundingBox.Draw(image, detectedFaces, boundingBoxColor, labelColor);

            using var outputObjectStream = new MemoryStream();
            image.Save(outputObjectStream, ImageFormat.GetObjectImageFormat(objectType));

            await _s3Service.PutObjectAsync(outputObjectStream);
            return _s3Service.GetPresignedUrl();
        }
    }
}