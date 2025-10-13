namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Key { get; set; }
        public required string FileName { get; set; }
        public string? ContentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UploadedById { get; set; }
        public AppUser? UploadedBy { get; set; }
    }
}
