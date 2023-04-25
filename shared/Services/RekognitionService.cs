using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using DetectifyLambdaServices.Services.Interfaces;

namespace DetectifyLambdaServices.Services
{
    public class RekognitionService : IRekognitionService
    {
        private readonly IServiceConfiguration _configuration;
        private readonly AmazonRekognitionClient _rekognitionClient;
        private float _confidencePercentage;

        public RekognitionService(IServiceConfiguration configuration)
        {
            _configuration = configuration;
            _rekognitionClient = new AmazonRekognitionClient();
        }

        public async Task<DetectLabelsResponse> DetectLabels(MemoryStream objectStream, float confidence)
        {
            var request = new DetectLabelsRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    Bytes = objectStream
                },
                MinConfidence = confidence
            };
            var response = await _rekognitionClient.DetectLabelsAsync(request);
            return response;
        }
        
        public async Task<RecognizeCelebritiesResponse> RecognizeCelebritiesSingleImage(MemoryStream objectStream)
        {
            var request = new RecognizeCelebritiesRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    Bytes = objectStream
                }
            };
            var response = await _rekognitionClient.RecognizeCelebritiesAsync(request);
            return response;
        }
    }
}