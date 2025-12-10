using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.ReportDtos
{
    public class RejectReportRequest
    {
        [Required(ErrorMessage = "Cần nhập lí do từ chối")]
        public string Reason { get; set; } = string.Empty;
    }
}
