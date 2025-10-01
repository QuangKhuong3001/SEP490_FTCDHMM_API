namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = "";
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UploadedById { get; set; }
        public AppUser UploadedBy { get; set; } = default!;
    }
}
