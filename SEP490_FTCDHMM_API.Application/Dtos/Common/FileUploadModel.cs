namespace SEP490_FTCDHMM_API.Application.Dtos.Common
{
    public class FileUploadModel
    {
        public string FileName { get; set; } = default!;
        public Stream Content { get; set; } = default!;
        public string ContentType { get; set; } = default!;
    }

}
