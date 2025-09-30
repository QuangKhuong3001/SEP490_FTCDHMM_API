using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class RolePermissionSettingDto
    {
        [Required(ErrorMessage = "Missing RoleId")]
        public List<PermissionToggleDto> Permissions { get; set; } = new();
    }
}
