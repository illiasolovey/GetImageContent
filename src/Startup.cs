using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectAnalysis.Models;
using ObjectAnalysis.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ObjectAnalysis
{
    class Startup
    {
        private ServiceCollection _serviceCollection;

        public Startup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            _serviceCollection = new ServiceCollection();
            ConfigureServices(configuration);
        }

        private void ConfigureServices(IConfiguration configuration)
        {
            _serviceCollection.AddSingleton(configuration);
            _serviceCollection.AddSingleton<IServiceConfiguration, ServiceConfiguration>();
            _serviceCollection.AddScoped<S3Service>();
            _serviceCollection.AddScoped<RekognitionService>();
            _serviceCollection.AddScoped<Functions>();
        }

        public Task<string> FunctionHandler(LambdaPayload payload, ILambdaContext context)
        {
            if (string.IsNullOrWhiteSpace(payload.ObjectKey))
                throw new ArgumentException("ObjectKey is a required property and cannot be null or empty.");
            using (ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider())
            {
                serviceProvider.GetService<ServiceConfiguration>();
                var s3Service = serviceProvider.GetService<S3Service>();
                s3Service!.SetObjectKey(payload.ObjectKey);
                serviceProvider.GetService<RekognitionService>();
                return serviceProvider.GetService<Functions>()!.ObjectAnalysis(payload.Confidence);
            }
        }
    }
}