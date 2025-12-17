using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class ChangeRoleRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn vai trò. ")]
        public Guid? RoleId { get; set; }

        [Required(ErrorMessage = "Cần xác định thời gian cuối cùng chỉnh sửa mục tiêu sức khỏe.")]
        public DateTime? LastUpdatedUtc { get; set; }
    }
}
