namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public Guid? UploadedById { get; set; }
        public AppUser? UploadedBy { get; set; }
    }
}
