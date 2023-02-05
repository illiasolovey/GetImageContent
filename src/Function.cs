using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.Extensions.Configuration;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GetImageContent;

public class Function
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

    public async Task<DetectLabelsResponse> FunctionHandler(string filename, ILambdaContext context)
    {
        string bucketName = FunctionConfiguration["LambdaConfiguration:BucketName"];
        var rekognitionClient = new AmazonRekognitionClient();
        var response = await rekognitionClient.DetectLabelsAsync(
            request: new DetectLabelsRequest
            {
                Image = new Image
                {
                    S3Object = new S3Object
                    {
                        Bucket = bucketName,
                        Name = filename
                    }
                }
            }
        );
        context.Logger.LogInformation($"API request at {DateTime.Now}. Requested file trace: \"{bucketName}\\{filename}\"");
        return response;
    }
}
