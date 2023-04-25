namespace DetectifyLambdaServices.Services.Interfaces
{
    public interface IServiceConfiguration
    {
        public string BucketGet { get; set; }
        public string BucketPut { get; set; }
        public int UrlLifetimeInMin { get; set; }
    }
}