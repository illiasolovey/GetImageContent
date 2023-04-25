using DetectifyLambdaServices.Services;
using DetectifyLambdaServices.Services.Interfaces;
using DetectifyLambdaServices.Utils;
using SixLabors.ImageSharp;

namespace ObjectAnalysis
{
    public class Function
    {
        private readonly S3Service _s3Service;
        private readonly IRekognitionService _rekognitionService;

        public Function(S3Service s3Service, IRekognitionService rekognitionService)
            => (_s3Service, _rekognitionService) = (s3Service, rekognitionService);

        public async Task<string> ObjectAnalysis(float confidence, string boundingBoxColor, string labelColor)
        {
            var objectStream = await _s3Service.GetObjectStreamAsync();
            var objectType = await _s3Service.GetObjectType();

            var detectedLabels = await _rekognitionService.DetectLabels(objectStream, confidence);

            var image = SixLabors.ImageSharp.Image.Load(objectStream);
            BoundingBox.Draw(image, detectedLabels, boundingBoxColor, labelColor);

            using var outputObjectStream = new MemoryStream();
            image.Save(outputObjectStream, ImageFormat.GetObjectImageFormat(objectType));

            await _s3Service.PutObjectAsync(outputObjectStream);
            return _s3Service.GetPresignedUrl();
        }
    }
}