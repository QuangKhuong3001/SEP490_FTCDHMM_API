namespace SEP490_FTCDHMM_API.Application.Dtos.LabelDtos
{
    public class LabelResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorCode { get; set; } = "#ffffff";
        public DateTime LastUpdatedUtc { get; set; }
    }
}
