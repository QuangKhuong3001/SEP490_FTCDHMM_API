using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RatingDtos
{
    public class RatingRequest
    {
        [Range(1, 5, ErrorMessage = "Điểm phải nằm trong khoảng giá trị từ 1 đến 5.")]
        public int Score { get; set; }

        [StringLength(256, MinimumLength = 1, ErrorMessage = "Đánh giá phải có độ dài từ 1 đến 256 ký tự.")]
        public string Feedback { get; set; } = string.Empty;
    }
}
