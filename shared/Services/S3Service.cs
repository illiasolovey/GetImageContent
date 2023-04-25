using Amazon.S3;
using Amazon.S3.Model;
using DetectifyLambdaServices.Services.Interfaces;

namespace DetectifyLambdaServices.Services
{
    public class S3Service
    {
        private readonly IServiceConfiguration _configuration;
        private readonly AmazonS3Client _s3Client;
        private GetObjectRequest _sharedObjectRequest = null!;
        private string _objectKey = null!;

        public S3Service(IServiceConfiguration configuration)
            => (_configuration, _s3Client) = (configuration, new AmazonS3Client());

        public void SetObjectKey(string objectKey)
        {
            _objectKey = objectKey;
            _sharedObjectRequest = new GetObjectRequest
            {
                BucketName = _configuration.BucketGet,
                Key = _objectKey
            };
        }

        public async Task<MemoryStream> GetObjectStreamAsync()
        {
            using var objectResponse = await _s3Client.GetObjectAsync(_sharedObjectRequest);
            using var objectMemoryStream = new MemoryStream();
            using (Stream responseStream = objectResponse.ResponseStream)
                await responseStream.CopyToAsync(objectMemoryStream);
            objectMemoryStream.Seek(0, SeekOrigin.Begin);
            return new MemoryStream(objectMemoryStream.ToArray());
        }

        public async Task<PutObjectResponse> PutObjectAsync(MemoryStream outputObjectStream)
        {
            string bucket = _configuration.BucketPut;
            return await _s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucket,
                Key = _objectKey,
                InputStream = outputObjectStream
            });
        }

        public async Task<string> GetObjectType()
        {
            using var objectResponse = await _s3Client.GetObjectAsync(_sharedObjectRequest);
            return objectResponse.Headers.ContentType;
        }

        public string GetPresignedUrl(int expiresInMin = 20)
        {
            string bucket = _configuration.BucketPut;
            DateTime expiration = DateTime.UtcNow.AddMinutes(_configuration.UrlLifetimeInMin);
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = _objectKey,
                Expires = expiration
            };
            return _s3Client.GetPreSignedURL(request);
        }
    }
}