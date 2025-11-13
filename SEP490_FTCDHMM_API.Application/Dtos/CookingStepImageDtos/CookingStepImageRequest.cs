using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.CookingStepImageDtos
{
    public class CookingStepImageRequest
    {
        public FileUploadModel Image { get; set; } = null!;
        public int ImageOrder { get; set; }
    }
}
