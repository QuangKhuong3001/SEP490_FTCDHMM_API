using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class RolePermissionSettingDto
    {
        [Required(ErrorMessage = "Missing RoleId")]
        public List<PermissionToggleRequest> Permissions { get; set; } = new();
    }
}
