using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDtos
{
    public class ChangeRoleRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn vai trò. ")]
        public Guid RoleId { get; set; }
    }
}
