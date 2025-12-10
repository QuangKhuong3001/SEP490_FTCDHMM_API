using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class CreateRole
    {
        [Required(ErrorMessage = "Nhập tên")]
        [StringLength(100, ErrorMessage = "Tên phải ít hơn 100 ký tự")]
        public string Name { get; set; } = string.Empty;
    }
}
