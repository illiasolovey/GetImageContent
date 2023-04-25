using Amazon.Rekognition.Model;

namespace DetectifyLambdaServices.Services.Interfaces
{
    public interface IRekognitionService
    {
        public Task<DetectLabelsResponse> DetectLabels(MemoryStream objectStream, float confidence);
        public Task<RecognizeCelebritiesResponse> RecognizeCelebritiesSingleImage(MemoryStream objectStream);
    }
}