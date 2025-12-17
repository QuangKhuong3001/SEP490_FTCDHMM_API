using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class RolePermissionSettingDto
    {
        [Required(ErrorMessage = "Thiếu  Permissions")]
        [MinLength(1, ErrorMessage = "Permissions phải chứa ít nhất 1 mục")]
        public List<PermissionToggleRequest> Permissions { get; set; } = new();

        [Required(ErrorMessage = "Cần xác định thời gian cuối cùng chỉnh sửa mục tiêu.")]
        public DateTime? LastUpdatedUtc { get; set; }
    }
}
