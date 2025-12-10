using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.ReportDtos
{
    public class CreateReportRequest
    {
        [Required(ErrorMessage = "Cần chỉ định mục tiêu")]
        public Guid TargetId { get; set; }

        [Required(ErrorMessage = "Cần chỉ định loại mục tiêu")]
        public string TargetType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
