using Microsoft.Extensions.Configuration;

namespace ObjectAnalysis.Services
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        private static IConfiguration _configuration = null!;
        private static string _nullDeserializationExceptionMessage = "Deserialization exception: configuration property is null.";

        public ServiceConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            BucketGet = _configuration["FunctionConfiguration:UploadBucketName"] ?? throw new Exception(_nullDeserializationExceptionMessage);
            BucketPut = _configuration["FunctionConfiguration:DownloadBucketName"] ?? throw new Exception(_nullDeserializationExceptionMessage);
            UrlLifetimeInMin = _configuration.GetValue<int>("FunctionConfiguration:UrlLifetimeInMin");
            Confidence = _configuration.GetValue<int>("FunctionConfiguration:Confidence");
        }

        public string BucketGet { get; set; }
        public string BucketPut { get; set; }
        public int UrlLifetimeInMin { get; set; }
        public int Confidence { get; set; }
    }
}