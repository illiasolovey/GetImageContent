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
            BucketGet = string.IsNullOrWhiteSpace(_configuration["StorageConfiguration:UploadBucketName"])
                ? throw new Exception(_nullDeserializationExceptionMessage)
                : _configuration["StorageConfiguration:UploadBucketName"];
            BucketPut = string.IsNullOrWhiteSpace(_configuration["StorageConfiguration:DownloadBucketName"])
                ? throw new Exception(_nullDeserializationExceptionMessage)
                : _configuration["StorageConfiguration:DownloadBucketName"];
            UrlLifetimeInMin = _configuration.GetValue<int>("FunctionConfiguration:UrlLifetimeInMin");
        }

        public string BucketGet { get; set; }
        public string BucketPut { get; set; }
        public int UrlLifetimeInMin { get; set; }
    }
}
