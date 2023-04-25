using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using DetectifyLambdaServices.Services;
using DetectifyShared.Models;
using DetectifyLambdaServices.DependencyInjection;
using DetectifyLambdaServices.Services.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ObjectAnalysis
{
    public class Startup
    {
        private readonly IServiceProvider _serviceProvider;

        public Startup()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.ConfigureDI();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public Task<string> FunctionHandler(LambdaPayload payload, ILambdaContext context)
        {
            if (string.IsNullOrWhiteSpace(payload.ObjectKey))
                throw new ArgumentException("ObjectKey is a required property and cannot be null or empty.");

            var s3Service = _serviceProvider.GetService<S3Service>();
            var rekognitionService = _serviceProvider.GetService<IRekognitionService>();

            if (s3Service == null || rekognitionService == null)
                throw new Exception("Unable to retrieve dependencies. Reference to service provider to resolve this issue.");

            s3Service.SetObjectKey(payload.ObjectKey);

            var function = new Function(s3Service, rekognitionService);
            return function.ObjectAnalysis(payload.Confidence, payload.BoundingBoxColorHEX!, payload.LabelColorHEX!);
        }
    }
}
