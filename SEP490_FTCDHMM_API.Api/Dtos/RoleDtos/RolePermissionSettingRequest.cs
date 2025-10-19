using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class RolePermissionSettingDto
    {
        [Required(ErrorMessage = "Missing Permissions")]
        public List<PermissionToggleRequest> Permissions { get; set; } = new();
    }
}
