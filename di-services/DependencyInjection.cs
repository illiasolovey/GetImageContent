using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DetectifyLambdaServices.Services;
using DetectifyLambdaServices.Services.Interfaces;

namespace DetectifyLambdaServices.DependencyInjection;

public static class DependencyInjection
{
    public static void ConfigureDI(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        services.AddSingleton(configuration);
        services.AddSingleton<IServiceConfiguration, ServiceConfiguration>();
        services.AddScoped<IRekognitionService, RekognitionService>();
        services.AddScoped<S3Service>();
    }
}
