namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Key { get; set; }
        public string? ContentType { get; set; }
        public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public Guid? UploadedById { get; set; }
        public AppUser? UploadedBy { get; set; }
    }
}
