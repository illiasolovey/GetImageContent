using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ObjectAnalysis
{
    class Function
    {
        public IConfiguration FunctionConfiguration { get; private set; } = null!;

        private void ConfigureSettings()
        {
            FunctionConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        public Function() =>
            ConfigureSettings();

        public async Task<string> FunctionHandler(string objectKey, ILambdaContext context)
        {
            string getBucket = FunctionConfiguration["StorageConfiguration:UploadBucketName"];
            using var s3Client = new AmazonS3Client();

            using var objectResponse = await s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = getBucket,
                Key = objectKey
            });
            string objectType = objectResponse.Headers.ContentType;

            using var objectStream = await GetObjectMemoryStream(objectResponse);
            DetectLabelsResponse detectedLabels = await DetectLabels(objectStream);

            byte[] objectStreamBytes = objectStream.ToArray();
            using var memoryStream = new MemoryStream(objectStreamBytes);
            using var image = SixLabors.ImageSharp.Image.Load(memoryStream);
            DrawBoundingBoxes(image, detectedLabels);
            using var outputObjectStream = new MemoryStream();
            image.Save(outputObjectStream, GetObjectImageFormat(objectType));

            string putBucket = FunctionConfiguration["StorageConfiguration:DownloadBucketName"];
            await s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = putBucket,
                Key = objectKey,
                InputStream = outputObjectStream
            });
            return GetPresignedUrl(
                s3Client: s3Client,
                bucketName: putBucket,
                objectKey: objectKey,
                expiresInMin: 20
            );
        }

        private async Task<MemoryStream> GetObjectMemoryStream(GetObjectResponse response)
        {
            using var objectMemoryStream = new MemoryStream();
            using (Stream responseStream = response.ResponseStream)
                await responseStream.CopyToAsync(objectMemoryStream);
            return objectMemoryStream;
        }

        private async Task<DetectLabelsResponse> DetectLabels(MemoryStream objectStream)
        {
            string bucketName = FunctionConfiguration["StorageConfiguration:UploadBucketName"];
            var rekognitionClient = new AmazonRekognitionClient();
            var response = await rekognitionClient.DetectLabelsAsync(
                request: new DetectLabelsRequest
                {
                    Image = new Amazon.Rekognition.Model.Image
                    {
                        Bytes = objectStream
                    },
                    MinConfidence = 70
                }
            );
            return response;
        }

        private void DrawBoundingBoxes(SixLabors.ImageSharp.Image image, DetectLabelsResponse detectResponse)
        {
            foreach (var label in detectResponse.Labels)
            {
                foreach (var instance in label.Instances)
                {
                    var bound = instance.BoundingBox;
                    int x = (int)(image.Width * bound.Left);
                    int y = (int)(image.Height * bound.Top);
                    int width = (int)(image.Width * bound.Width);
                    int height = (int)(image.Height * bound.Height);
                    var rectangle = new Rectangle(x, y, width, height);
                    var points = new PointF(x, y);
                    DrawBoundingRectangle(image, label, rectangle, points);
                }
            }
        }

        private void DrawBoundingRectangle(SixLabors.ImageSharp.Image image, Label label, Rectangle rectangle, PointF points)
        {
            var labelName = label.Name;
            var confidence = Math.Round(label.Confidence, 2);
            var text = $"{labelName} ({confidence * 100}%)";
            var pen = new Pen(Color.Red, 5);
            var font = SystemFonts.CreateFont("DejaVu Sans", 20, FontStyle.Bold);
            var color = new SixLabors.ImageSharp.PixelFormats.Rgba32(255, 255, 255);
            var brush = Brushes.Solid(color);
            image.Mutate(ctx => ctx
                .Draw(pen, rectangle)
                .DrawText(label.Name, font, brush, points)
            );
        }

        private IImageFormat GetObjectImageFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/jpg":
                case "image/jpeg":
                    return SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance;
                case "image/png":
                    return SixLabors.ImageSharp.Formats.Png.PngFormat.Instance;
                case "image/gif":
                    return SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance;
                case "image/bmp":
                    return SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance;
                default:
                    throw new ArgumentException($"{contentType} is not supported.");
            }
        }

        private string GetPresignedUrl(AmazonS3Client s3Client, string bucketName, string objectKey, int expiresInMin)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(expiresInMin);
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = expiration
            };
            return s3Client.GetPreSignedURL(request);
        }
    }
}