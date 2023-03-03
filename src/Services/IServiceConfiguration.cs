namespace ObjectAnalysis.Services
{
    public interface IServiceConfiguration
    {
        public string BucketGet { get; set; }
        public string BucketPut { get; set; }
        public int UrlLifetimeInMin { get; set; }
        public int Confidence { get; set; }
    }
}