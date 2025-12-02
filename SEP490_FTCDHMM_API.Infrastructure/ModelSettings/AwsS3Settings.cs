namespace SEP490_FTCDHMM_API.Infrastructure.ModelSettings
{
    public class AwsS3Settings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public int ExpiryDays = 1;
    }
}
