namespace SEP490_FTCDHMM_API.Application.Dtos.LabelDtos
{
    public class UpdateColorCodeRequest
    {
        public DateTime? LastUpdatedUtc { get; set; }
        public string ColorCode { get; set; } = string.Empty;
    }
}
