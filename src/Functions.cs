using ObjectAnalysis.Services;
using SixLabors.ImageSharp;

namespace ObjectAnalysis
{
    public class Functions
    {
        private readonly S3Service _s3Service;
        private readonly RekognitionService _rekognitionService;

        public Functions(S3Service s3Service, RekognitionService rekognitionService)
            => (_s3Service, _rekognitionService) = (s3Service, rekognitionService);

        public async Task<string> ObjectAnalysis()
        {
            var objectStream = await _s3Service.GetObjectStreamAsync();
            var objectType = await _s3Service.GetObjectType();
            var detectedLabels = await _rekognitionService.DetectLabels(objectStream);

            byte[] objectStreamBytes = objectStream.ToArray();
            using var memoryStream = new MemoryStream(objectStreamBytes);
            using var image = SixLabors.ImageSharp.Image.Load(memoryStream);
            Utils.BoundingBox.Draw(image, detectedLabels);

            using var outputObjectStream = new MemoryStream();
            image.Save(outputObjectStream, Utils.ImageFormat.GetObjectImageFormat(objectType));

            await _s3Service.PutObjectAsync(outputObjectStream);
            return _s3Service.GetPresignedUrl(expiresInMin: 20);
        }
    }
}